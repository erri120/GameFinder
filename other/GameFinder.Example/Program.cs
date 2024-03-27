using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using CommandLine;
using Microsoft.Extensions.Logging;
using NexusMods.Paths;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using FileSystem = NexusMods.Paths.FileSystem;
using ILogger = Microsoft.Extensions.Logging.ILogger;

[assembly: ExcludeFromCodeCoverage]
namespace GameFinder.Example;

public static class Program
{
    private static NLogLoggerProvider _provider = null!;

    public static void Main(string[] args)
    {
        var config = new LoggingConfiguration();

        var coloredConsoleTarget = new ColoredConsoleTarget("coloredConsole")
        {
            DetectConsoleAvailable = true,
            EnableAnsiOutput = OperatingSystem.IsLinux(), // windows hates this
            UseDefaultRowHighlightingRules = false,
            WordHighlightingRules =
            {
                new ConsoleWordHighlightingRule
                {
                    Regex = @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}\.\d+",
                    CompileRegex = true,
                    ForegroundColor = ConsoleOutputColor.Gray,
                },
                new ConsoleWordHighlightingRule("DEBUG", ConsoleOutputColor.Gray, ConsoleOutputColor.NoChange),
                new ConsoleWordHighlightingRule("INFO", ConsoleOutputColor.Cyan, ConsoleOutputColor.NoChange),
                new ConsoleWordHighlightingRule("ERROR", ConsoleOutputColor.Red, ConsoleOutputColor.NoChange),
                new ConsoleWordHighlightingRule("WARNING", ConsoleOutputColor.Yellow, ConsoleOutputColor.NoChange),
            },
            Layout = "${longdate}|${level:uppercase=true}|${logger}|${message:withexception=true}",
        };

        var fileTarget = new FileTarget("file")
        {
            FileName = "log.log",
        };

        config.AddRuleForAllLevels(coloredConsoleTarget);
        config.AddRuleForAllLevels(fileTarget);

        LogManager.Configuration = config;
        _provider = new NLogLoggerProvider();

        var logger = _provider.CreateLogger(nameof(Program));

        Parser.Default
            .ParseArguments<Options>(args)
            .WithParsed(x => Run(x, logger));
    }

    private static void Run(Options options, ILogger logger)
    {
        var realFileSystem = FileSystem.Shared;

        var logFile = realFileSystem.GetKnownPath(KnownPath.CurrentDirectory).Combine("log.log");
        if (realFileSystem.FileExists(logFile)) realFileSystem.DeleteFile(logFile);

        logger.LogInformation("Operating System: {OSDescription}", RuntimeInformation.OSDescription);
    }
}
