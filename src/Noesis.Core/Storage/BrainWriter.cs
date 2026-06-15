using Noesis.Core.Models;
using Microsoft.Data.Sqlite;

namespace Noesis.Core.Storage;

public class BrainWriter
{
    private readonly string _brainPath;
    private readonly string _dbPath;

    public BrainWriter(string brainPath, string dbPath)
    {
        this._brainPath = brainPath;
        this._dbPath = dbPath;
    }
    
    //TODO: Validator call will be here once I get to Phase 0.5
    
    private string RenderDecisionMarkdown(Decision decision)
    {
        return $@"---
ID: {decision.Id}
Type: {decision.Type}
ProjectId: {decision.ProjectId}
Confidence: {decision.Confidence}
CreatedAt: {decision.CreatedAt}
Source: {decision.Source ?? "Unknown"}
---

# {decision.Content}

### Reason:
**{decision.Reason}**

### Alternatives Considered:
- {String.Join("\n- ", decision.Alternatives)}

### Status: 
{decision.Status.ToString()}";
    }

    public async Task WriteDecisionAsync(Decision decision)
    {
        string timestamp = DateTimeOffset.UtcNow.ToString("yyyyMMddHHmmss");
        string filename = timestamp + "-" + decision.Id + ".md";
        string directory = Path.Combine(_brainPath, "decisions");
        string fullpath =  Path.Combine(directory, filename);
        Directory.CreateDirectory(directory);
        
        string markdownContent = RenderDecisionMarkdown(decision);
        await File.WriteAllTextAsync(fullpath, markdownContent);
        
        using var connection = new SqliteConnection($"Data Source={_dbPath};");
        await connection.OpenAsync();

        try
        {
            await InsertIntoFtsAsync(connection, decision, fullpath);
        }
        catch
        {
            try
            {
                File.Delete(fullpath);
            }
            catch{}

            throw;
        }
    }
    
    private async Task InsertIntoFtsAsync(SqliteConnection connection, MemoryEntry entry, string filePath)
    {
        using var transaction = await connection.BeginTransactionAsync();
        using var command = connection.CreateCommand();

        command.Transaction = (SqliteTransaction)transaction;

        command.CommandText = @"INSERT INTO memory (id,
                    type, 
                    project_id, 
                    content, 
                    confidence, 
                    created_at, 
                    file_path) VALUES (@id,
                                       @type,
                                       @project_id,
                                       @content,
                                       @confidence,
                                       @created_at,
                                       @file_path);";
        
        command.Parameters.AddWithValue("@id", entry.Id.ToString());
        command.Parameters.AddWithValue("@type", entry.Type.ToString());
        command.Parameters.AddWithValue("@project_id", entry.ProjectId.ToString());
        command.Parameters.AddWithValue("@content", entry.Content);
        command.Parameters.AddWithValue("@confidence", entry.Confidence.ToString());
        command.Parameters.AddWithValue("@created_at", entry.CreatedAt.ToString("o"));
        command.Parameters.AddWithValue("@file_path", filePath);
        
        await command.ExecuteNonQueryAsync();
        await transaction.CommitAsync();
    }

    public async Task WriteEntityAsync(Entity entity)
    {
        string timestamp =  DateTimeOffset.UtcNow.ToString("yyyyMMddHHmmss");
        string filename = timestamp + "-" + entity.Id.ToString() + ".md";
        string directory = Path.Combine(_brainPath, "entities");
        string fullpath = Path.Combine(directory, filename);
        Directory.CreateDirectory(directory);

        string markdownContent = RenderEntityMarkdown(entity);
        await File.WriteAllTextAsync(fullpath, markdownContent);
        
        using var connection = new SqliteConnection($"Data Source={_dbPath};");
        await connection.OpenAsync();

        try
        {
            await InsertIntoFtsAsync(connection, entity, fullpath);
        }
        catch
        {
            try
            {
                File.Delete(fullpath);
            }
            catch {}

            throw;
        }
    }

    private string RenderEntityMarkdown(Entity entity)
    {
        return $@"---
ID: {entity.Id}
Type: {entity.Type}
CurrentEntityType: {entity.CurrentEntityType.ToString()}
ProjectId: {entity.ProjectId}
Confidence: {entity.Confidence}
Source: {entity.Source ?? "Unknown"}
File: {entity.Location ?? "Unknown"}
---

# {entity.Content}

### Entity description:
{entity.Description}";
    }
    
    public async Task WriteRawAsync(RawMemory rawMemory)
    {
        
    }
    
    public async Task WriteLessonAsync(Lesson lesson)
    {
        
    }
}