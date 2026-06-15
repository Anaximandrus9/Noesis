using Microsoft.Data.Sqlite;
using System.Threading.Tasks;
namespace Noesis.Core.Storage;

public class FtsIndex
{
    public async Task InitializeAsync(string dbPath)
    {
        using var connection = new SqliteConnection($"Data Source={dbPath};");
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        
        command.CommandText = @"
        CREATE VIRTUAL TABLE IF NOT EXISTS memory USING fts5(
            id UNINDEXED,
            type,
            project_id,
            content,
            confidence,
            created_at UNINDEXED,
            file_path UNINDEXED,
            tokenize = 'porter ascii'
        );";
        
        await command.ExecuteNonQueryAsync();
    }
}

