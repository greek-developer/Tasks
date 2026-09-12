using System.CommandLine;

namespace Tasks.Versioning;

internal static class VersionCommand
{
    internal static Command Create()
    {
        var command = new Command("version", "Print the version, commit and build time of this build");
        command.SetAction(Run);
        return command;
    }

    private static int Run(ParseResult _)
    {
        var path = Path.Combine(AppContext.BaseDirectory, ProductionVersion.FileName);

        if (!File.Exists(path))
        {
            Console.Error.WriteLine(
                $"{ProductionVersion.FileName} is missing from '{AppContext.BaseDirectory}'. It is written at build time.");
            return 1;
        }

        var version = ProductionVersion.Parse(File.ReadAllText(path));
        if (!version.IsValid)
        {
            foreach (var error in version.Errors)
            {
                Console.Error.WriteLine(error);
            }

            return 1;
        }

        // The version block is the four lines and nothing else, so it can be read by a
        // machine; the update hint follows a blank line after it.
        Console.Out.WriteLine(version.Value!.ToDisplay());
        Console.Out.WriteLine();
        Console.Out.WriteLine("To update: dotnet tool update -g grdev.tasks-cli");
        return 0;
    }
}
