using System.CommandLine;

RootCommand rootCommand = new("Noesis - shared long-term memory OS for LLMs");

Command initCommand = new("init", "Initialises the Noesis brain folder structure and database");
rootCommand.Subcommands.Add(initCommand);

initCommand.SetAction(parseResult =>
{
    string repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
    string brainPath = Path.Combine(repoRoot, "brain"); //TODO: replace once config loading exists
    string[] items =
        { "raw", "knowledge", "decisions", "entities", "relationships", "manifests", "lessons", "patterns" };

    int index = 0;
    foreach (string item in items)
    {
        string directoryPath = Path.Combine(brainPath, item);
        Directory.CreateDirectory(directoryPath);
        
        if(Directory.Exists(directoryPath))
            Console.WriteLine($"Directory {item} ({++index}) ensured at {directoryPath}");
    }

}
return rootCommand.Parse(args).Invoke();
