using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using CommandLine;
using GameFinder.Common;
using GameFinder.RegistryUtils;
using GameFinder.StoreHandlers.GOG;
using GameFinder.StoreHandlers.Origin;
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
                new ConsoleWordHighlightingRule("TRACE", ConsoleOutputColor.Gray, ConsoleOutputColor.NoChange),
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
        ILoggerFactory loggerFactory = new NLogLoggerFactory();

        Parser.Default
            .ParseArguments<Options>(args)
            .WithParsed(x => Run(x, loggerFactory));
    }

    private static void Run(Options options, ILoggerFactory loggerFactory)
    {
        var realFileSystem = FileSystem.Shared;

        var logFile = realFileSystem.GetKnownPath(KnownPath.CurrentDirectory).Combine("log.log");
        if (realFileSystem.FileExists(logFile)) realFileSystem.DeleteFile(logFile);

        var logger = loggerFactory.CreateLogger(nameof(Program));
        logger.LogInformation("Operating System: {OSDescription}", RuntimeInformation.OSDescription);

        // TODO: Linux and macOS
        if (!OperatingSystem.IsWindows()) return;

        var gameFinder = GameFinderBuilder.Create(
            loggerFactory,
            new OriginHandler(loggerFactory, realFileSystem),
            new GOGHandler(loggerFactory, realFileSystem, new WindowsRegistry())
        );

        var foundGames = gameFinder.FindAllGames();
    }
}
