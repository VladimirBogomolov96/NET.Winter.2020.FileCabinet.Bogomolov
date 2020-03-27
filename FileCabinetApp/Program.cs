using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using CommandLine;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.Printers;
using FileCabinetApp.Services;
using FileCabinetApp.Validators;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp
{
    /// <summary>
    /// API class of the program.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Vladimir Bogomolov";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private static IFileCabinetService fileCabinetService;
        private static bool isRunning = true;

        /// <summary>
        /// Point of entrance to program.
        /// </summary>
        /// <param name="args">Command prompt arguments.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            SetCommandLineSettings(args);
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();
            var commands = CreateCommandHandlers();
            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                const int argumentIndex = 1;
                var command = inputs[commandIndex];
                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                var parameters = inputs.Length > 1 ? inputs[argumentIndex] : string.Empty;
                commands.Handle(new AppCommandRequest(command, parameters));
            }
            while (isRunning);
        }

        private static void SetCommandLineSettings(string[] args)
        {
            if (!File.Exists("D:\\EPAM\\internship\\FileCabinet\\FileCabinetApp\\Validators\\validation-rules.json"))
            {
                Console.WriteLine("Can't find validation-rules.json file.");
                Environment.Exit(-1);
            }

            IConfigurationRoot configuration = null;
            try
            {
                configuration = new ConfigurationBuilder()
                   .SetBasePath("D:\\EPAM\\internship\\FileCabinet\\FileCabinetApp\\Validators")
                   .AddJsonFile("validation-rules.json")
                   .Build();
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid data in validation-rules.json file.");
                Environment.Exit(-1);
            }

            Options options = new Options();
            var result = Parser.Default.ParseArguments<Options>(args).WithParsed(parsed => options = parsed);
            if (options.Storage.Equals("file", StringComparison.InvariantCultureIgnoreCase))
            {
                SetFileService();
            }
            else if (options.Storage.Equals("memory", StringComparison.InvariantCultureIgnoreCase))
            {
                SetMemoryService();
            }
            else
            {
                throw new ArgumentException("Wrong command line argument.", nameof(args));
            }

            if (options.Rule.Equals("custom", StringComparison.InvariantCultureIgnoreCase))
            {
                SetCustomService(configuration);
            }
            else if (options.Rule.Equals("default", StringComparison.InvariantCultureIgnoreCase))
            {
                SetDefaultService(configuration);
            }
            else
            {
                throw new ArgumentException("Wrong command line argument.", nameof(args));
            }

            if (options.Logger)
            {
                SetLogger();
            }

            if (options.Stopwatch)
            {
                SetStopwatch();
            }
        }

        private static void SetMemoryService()
        {
            fileCabinetService = new FileCabinetMemoryService();
            Console.WriteLine("Using memory service.");
        }

        private static void SetFileService()
        {
            FileStream fileStream = new FileStream("cabinet-records.db", FileMode.Create, FileAccess.ReadWrite);
            fileCabinetService = new FileCabinetFilesystemService(fileStream);
            Console.WriteLine("Using file service.");
        }

        private static void SetDefaultService(IConfigurationRoot configuration)
        {
            fileCabinetService.SetRecordValidator(new ValidatorBuilder().CreateValidator(configuration.GetSection("default")));
            Console.WriteLine("Using default validation rules.");
        }

        private static void SetCustomService(IConfigurationRoot configuration)
        {
            fileCabinetService.SetRecordValidator(new ValidatorBuilder().CreateValidator(configuration.GetSection("custom")));
            Console.WriteLine("Using custom validation rules.");
        }

        private static void SetStopwatch()
        {
            fileCabinetService = new ServiceMeter(fileCabinetService);
            Console.WriteLine("Using stopwatch.");
        }

        private static void SetLogger()
        {
            fileCabinetService = new ServiceLogger(fileCabinetService);
            Console.WriteLine("Using logger.");
        }

        private static void IsRunning(bool state)
        {
            isRunning = state;
        }

        private static ICommandHandler CreateCommandHandlers()
        {
            ICommandHandler createHandler = new CreateCommandHandler(fileCabinetService);
            ICommandHandler insertHandler = new InsertCommandHandler(fileCabinetService);
            ICommandHandler exitHandler = new ExitCommandHandler(IsRunning);
            ICommandHandler exportHandler = new ExportCommandHandler(fileCabinetService);
            ICommandHandler helpHandler = new HelpCommandHandler();
            ICommandHandler importHandler = new ImportCommandHandler(fileCabinetService);
            ICommandHandler purgeHandler = new PurgeCommandHandler(fileCabinetService);
            ICommandHandler statHandler = new StatCommandHandler(fileCabinetService);
            ICommandHandler deleteHandler = new DeleteCommandHandler(fileCabinetService);
            ICommandHandler updateHandler = new UpdateCommandHandler(fileCabinetService);
            ICommandHandler selectHandler = new SelectCommandHandler(fileCabinetService);
            helpHandler.SetNext(createHandler).
                SetNext(insertHandler).
                SetNext(importHandler).
                SetNext(exportHandler).
                SetNext(updateHandler).
                SetNext(statHandler).
                SetNext(selectHandler).
                SetNext(deleteHandler).
                SetNext(purgeHandler).
                SetNext(exitHandler);
            return helpHandler;
        }
    }
}