using System;
using System.Collections.Generic;
using System.IO;
using FileCabinetApp.CommandHandlers;
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

            Options options = GetCommandLineArguments(args);
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
                Console.WriteLine($"Wrong command line argument {options.Storage}.");
                Environment.Exit(-1);
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
                Console.WriteLine($"Wrong command line argument {options.Rule}.");
                Environment.Exit(-1);
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

        private static void ChangeProgramState(bool state)
        {
            isRunning = state;
        }

        private static ICommandHandler CreateCommandHandlers()
        {
            ICommandHandler createHandler = new CreateCommandHandler(fileCabinetService);
            ICommandHandler insertHandler = new InsertCommandHandler(fileCabinetService);
            ICommandHandler exitHandler = new ExitCommandHandler(ChangeProgramState);
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

        private static Options GetCommandLineArguments(string[] args)
        {
            List<string> singleParams = new List<string>();
            List<string> doubleParams = new List<string>();
            List<string> doubleParamsValues = new List<string>();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Substring(0, 2) == "--")
                {
                    singleParams.Add(args[i]);
                    continue;
                }
                else if (args[i].Substring(0, 1) == "-")
                {
                    if (i == (args.Length - 1))
                    {
                        Console.WriteLine($"Invalid command line parameter '{args[i]}'.");
                        Environment.Exit(-1);
                    }

                    doubleParams.Add(args[i]);
                    doubleParamsValues.Add(args[i + 1]);
                    i++;
                    continue;
                }
                else
                {
                    Console.WriteLine($"Invalid command line parameter '{args[i]}'.");
                    Environment.Exit(-1);
                }
            }

            Options options = new Options();
            foreach (string param in singleParams)
            {
                string[] keyValuePair = param.Split('=');
                if (keyValuePair.Length == 1)
                {
                    if (keyValuePair[0] == "--use-stopwatch")
                    {
                        options.Stopwatch = true;
                    }
                    else if (keyValuePair[0] == "--use-logger")
                    {
                        options.Logger = true;
                    }
                    else
                    {
                        Console.WriteLine($"Invalid command line parameter '{param}'.");
                        Environment.Exit(-1);
                    }
                }
                else if (keyValuePair.Length == 2)
                {
                    if (keyValuePair[0] == "--validation-rules")
                    {
                        options.Rule = keyValuePair[1];
                    }
                    else if (keyValuePair[0] == "--storage")
                    {
                        options.Storage = keyValuePair[1];
                    }
                    else
                    {
                        Console.WriteLine($"Invalid command line parameter '{param}'.");
                        Environment.Exit(-1);
                    }
                }
                else
                {
                    Console.WriteLine($"Invalid command line parameter '{param}'.");
                    Environment.Exit(-1);
                }
            }

            for (int i = 0; i < doubleParams.Count; i++)
            {
                if (doubleParams[i] == "-v")
                {
                    options.Rule = doubleParamsValues[i];
                }
                else if (doubleParams[i] == "-s")
                {
                    options.Storage = doubleParamsValues[i];
                }
                else
                {
                    Console.WriteLine($"Invalid command line parameter '{doubleParams[i]}'.");
                    Environment.Exit(-1);
                }
            }

            return options;
        }
    }
}