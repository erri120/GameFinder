using CommandLine;

namespace GameFinder.Example;

public class Options
{
    [Option("egs", HelpText = "Search for EGS games")]
    public bool EGS { get; set; } = true;

    [Option("gog", HelpText = "Search for GOG games")]
    public bool GOG { get; set; } = true;

    [Option("steam", HelpText = "Search for Steam games")]
    public bool Steam { get; set; } = true;

    [Option("origin", HelpText = "Search for Origin games")]
    public bool Origin { get; set; } = true;

    [Option("ea_desktop", HelpText = "Search for EA Desktop games")]
    public bool EADesktop { get; set; } = true;

    [Option("xbox", HelpText = "Search for Xbox Games Pass games")]
    public bool Xbox { get; set; } = true;

    [Option("wine", HelpText = "Search for wine prefixes")]
    public bool Wine { get; set; } = true;

    [Option("bottles", HelpText = "Search for wine prefixes managed with bottles")]
    public bool Bottles { get; set; } = true;

    [Option("log", HelpText = $"Log level (0-6; 0=all messages, 6=none")]
    public int LogLevel { get; set; } = -1;
}
