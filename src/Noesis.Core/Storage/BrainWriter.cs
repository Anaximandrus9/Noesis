using Noesis.Core.Models;
using Microsoft.Data.Sqlite;

namespace Noesis.Core.Storage;


//Per WriteAsync() methods use:
// string timestamp
//string filename
//string directory
//string fullpath

public class BrainWriter
{
    private readonly string _brainPath;
    private readonly string _dbPath;
    
    public BrainWriter(string brainPath, string dbPath)
    {
        this._brainPath = brainPath;
        this._dbPath = dbPath;
    }
    
    
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
        ValidationResult result = Validator.Validate(decision);
        if (!result.IsSuccess)
            throw new ArgumentException($"Invalid memory entry\n{String.Join(",", result.Errors)}");
        
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
        ValidationResult result = Validator.Validate(entity);
        if (!result.IsSuccess)
            throw new ArgumentException($"Invalid memory entry\n{String.Join(",", result.Errors)}");
        
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
CreatedAt:  {entity.CreatedAt}
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


    private string RelatedDecisionsFallback(in List<string>? decisions) // (*)
    {
        if (decisions is null or [])
            return "None Available";
        return String.Join("\n- ",  decisions);
    }
    private string RenderLessonMarkdown(Lesson lesson)
    {
        return $@"---
ID: {lesson.Id}
CreatedAt:  {lesson.CreatedAt}
ProjectId: {lesson.ProjectId}
Content: {lesson.Content}
Confidence: {lesson.Confidence}
Type: {lesson.Type}
Source: {lesson.Source ?? "Unknown"}
---

# {lesson.Content}

### Observed In:
- {String.Join("\n- ", lesson.ObservedIn)}

### Related Decisions:
- {RelatedDecisionsFallback(lesson.RelatedDecisions)}"; // (*) fallback: if lesson.RelatedDecisions is null, String.Join can't handle it.
        
    }
    
    public async Task WriteLessonAsync(Lesson lesson)
    {
        ValidationResult result = Validator.Validate(lesson);
        if (!result.IsSuccess)
            throw new ArgumentException($"Invalid memory entry\n{String.Join(",", result.Errors)}");
        
        string timestamp = DateTimeOffset.UtcNow.ToString("yyyyMMddHHmmss");
        string filename = timestamp + "-" + lesson.Id.ToString() + ".md";
        string directory = Path.Combine(_brainPath, "lessons");
        string fullpath = Path.Combine(directory, filename);
        Directory.CreateDirectory(directory);
        
        string markdownContent = RenderLessonMarkdown(lesson);
        await File.WriteAllTextAsync(fullpath, markdownContent);

        using var connection = new SqliteConnection($"Data Source={_dbPath};");
        await connection.OpenAsync();

        try
        {
            await InsertIntoFtsAsync(connection, lesson, fullpath);
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

    public async Task WriteRawAsync(RawMemory rawMemory)
    {
        ValidationResult result = Validator.Validate(rawMemory);
        if (!result.IsSuccess)
            throw new ArgumentException($"Invalid memory entry\n{String.Join(",", result.Errors)}");
        
        string timestamp = DateTimeOffset.UtcNow.ToString("yyyyMMddHHmmss");
        string filename = timestamp + "-" + rawMemory.Id.ToString() + ".md";
        string directory = Path.Combine(_brainPath, "raw");
        string fullpath =  Path.Combine(directory, filename);
        Directory.CreateDirectory(directory);

        string markdownContent = RenderRawMarkdown(rawMemory);
        await File.WriteAllTextAsync(fullpath, markdownContent);
        
        using var connection = new SqliteConnection($"Data Source={_dbPath};");
        await connection.OpenAsync();

        try
        {
            await InsertIntoFtsAsync(connection, rawMemory, fullpath);
        }
        catch
        {
            try
            {
                File.Delete(fullpath);
            } catch {}

            throw;
        }
    }

    private string RenderRawMarkdown(RawMemory rawMemory)
    {
        string humanReadableTime = RenderHumanReadableTime(rawMemory.SessionDurationSeconds); 
        // (**) Convert rawMemory.SessionDurationSeconds to an actual human-readable format

        return $@"---
ID: {rawMemory.Id}
CreatedAt:  {rawMemory.CreatedAt}
ProjectId: {rawMemory.ProjectId}
Content:  {rawMemory.Content}
Confidence: {rawMemory.Confidence}
Type:  {rawMemory.Type}
Source: {rawMemory.Source ?? "Unknown"}
ToolName: {rawMemory.ToolName}
SessionDuration: {humanReadableTime} 
---

# {rawMemory.Content}";

    }

    private string RenderHumanReadableTime(in int sessionSeconds) // (**)
    {
        int minutes = (sessionSeconds % 3600) / 60;
        int hours = sessionSeconds / 3600;

        return $"{hours}:{minutes}:{sessionSeconds % 60}";
    }
}