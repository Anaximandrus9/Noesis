using System.CommandLine;
using Noesis.Core.Storage;
using System.Collections.Generic;

string repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
string brainPath = Path.Combine(repoRoot, "brain"); //TODO: replace once config loading exists
string dbPath = Path.Combine(repoRoot, "noesis.db");

RootCommand rootCommand = new("Noesis - shared long-term memory OS for LLMs");
Command initCommand = new("init", "Initialises the Noesis brain folder structure and database");
Command rememberCommand = new("remember", "Records a new decision into the brain");

initCommand.SetAction(async parseResult =>
{
    string[] items =
        { "raw", "knowledge", "decisions", "entities", "relationships", "manifests", "lessons", "patterns" };

    FtsIndex ftsIndex = new FtsIndex();
    await ftsIndex.InitializeAsync(dbPath);
    if (File.Exists(dbPath))
        Console.WriteLine($"Database successfully initialized in {dbPath}");

    int index = 0;
    foreach (string item in items)
    {
        string directoryPath = Path.Combine(brainPath, item);
        Directory.CreateDirectory(directoryPath);

        if (Directory.Exists(directoryPath))
            Console.WriteLine($"Directory {item} ({++index}) ensured at {directoryPath}");
    }

});

Option<string> contentOption = new("--content")
{
    Description = "The decision being recorded"
};
Option<string> reasonOption = new("--reason")
{
    Description = "Why the decision was made"
};

rememberCommand.SetAction(async parseResult =>
{
    string? content = parseResult.GetValue(contentOption);
    string? reason = parseResult.GetValue(reasonOption);
    
    Console.WriteLine($"{content ?? "No content available"}\n{reason ?? "No reason available"}");
});

rootCommand.Subcommands.Add(initCommand);
rememberCommand.Options.Add(contentOption);
rememberCommand.Options.Add(reasonOption);
rootCommand.Subcommands.Add(rememberCommand);

return await rootCommand.Parse(args).InvokeAsync();
