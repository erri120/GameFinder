using CommandLine;

namespace GameFinder.Example
{
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
    }
}
