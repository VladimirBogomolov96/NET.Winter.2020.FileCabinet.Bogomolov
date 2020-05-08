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
        private static IFileCabinetService fileCabinetService;
        private static bool isRunning = true;

        /// <summary>
        /// Point of entrance to program.
        /// </summary>
        /// <param name="args">Command prompt arguments.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine(Configurator.GetConstantString("HelloMessage"));
            SetCommandLineSettings(args);
            Console.WriteLine(Configurator.GetConstantString("HintMessage"));
            Console.WriteLine();
            var commands = CreateCommandHandlers();
            do
            {
                Console.Write(Configurator.GetConstantString("CommandStartChar"));
                var inputs = Console.ReadLine().Trim().Split(' ', 2);
                const int commandIndex = 0;
                const int argumentIndex = 1;
                var command = inputs[commandIndex].Trim();
                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Configurator.GetConstantString("HintMessage"));
                    continue;
                }

                var parameters = inputs.Length > 1 ? inputs[argumentIndex].Trim() : string.Empty;
                commands.Handle(new AppCommandRequest(command, parameters));
            }
            while (isRunning);
        }

        private static void SetCommandLineSettings(string[] args)
        {
            IConfigurationRoot validationRules = null;
            try
            {
                validationRules = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile(Configurator.GetSetting("ValidationRulesFileName"))
                    .Build();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"{Configurator.GetConstantString("MissingValidation")} {Configurator.GetSetting("ConstantStringsFileName")}");
                Console.WriteLine(Configurator.GetConstantString("ClosingProgram"));
                Environment.Exit(-1);
            }
            catch (FormatException)
            {
                Console.WriteLine(Configurator.GetConstantString("InvalidValidationFile"));
                Console.WriteLine(Configurator.GetConstantString("ClosingProgram"));
                Environment.Exit(-1);
            }

            Options options = GetCommandLineArguments(args);
            if (options.Storage.Equals(Configurator.GetConstantString("FileStorage"), StringComparison.InvariantCultureIgnoreCase))
            {
                SetFileService();
            }
            else if (options.Storage.Equals(Configurator.GetConstantString("MemoryStorage"), StringComparison.InvariantCultureIgnoreCase))
            {
                SetMemoryService();
            }
            else
            {
                Console.WriteLine($"Wrong command line argument {options.Storage}.");
                Environment.Exit(-1);
            }

            if (options.Rule.Equals(Configurator.GetConstantString("CustomRule"), StringComparison.InvariantCultureIgnoreCase))
            {
                SetCustomService(validationRules);
            }
            else if (options.Rule.Equals(Configurator.GetConstantString("DefaultRule"), StringComparison.InvariantCultureIgnoreCase))
            {
                SetDefaultService(validationRules);
            }
            else
            {
                Console.WriteLine($"Wrong command line argument {options.Rule}.");
                Environment.Exit(-1);
            }

            if (fileCabinetService is FileCabinetFilesystemService)
            {
                try
                {
                    fileCabinetService.Restore(fileCabinetService.MakeSnapshot());
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    Console.WriteLine(Configurator.GetConstantString("InvalidFile"));
                    Console.WriteLine(Configurator.GetConstantString("ClosingProgram"));
                    Environment.Exit(-1);
                }
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
            Console.WriteLine(Configurator.GetConstantString("UseMemory"));
        }

        private static void SetFileService()
        {
            FileStream fileStream = new FileStream(Configurator.GetConstantString("DefaultFilesystemFileName"), FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fileCabinetService = new FileCabinetFilesystemService(fileStream);
            Console.WriteLine(Configurator.GetConstantString("UseFile"));
        }

        private static void SetDefaultService(IConfigurationRoot configuration)
        {
            fileCabinetService.SetRecordValidator(new ValidatorBuilder().CreateValidator(configuration.GetSection("default")));
            Console.WriteLine(Configurator.GetConstantString("UseDefaultRules"));
        }

        private static void SetCustomService(IConfigurationRoot configuration)
        {
            fileCabinetService.SetRecordValidator(new ValidatorBuilder().CreateValidator(configuration.GetSection("custom")));
            Console.WriteLine(Configurator.GetConstantString("UseCustomRules"));
        }

        private static void SetStopwatch()
        {
            fileCabinetService = new ServiceMeter(fileCabinetService);
            Console.WriteLine(Configurator.GetConstantString("UseStopwatch"));
        }

        private static void SetLogger()
        {
            fileCabinetService = new ServiceLogger(fileCabinetService);
            Console.WriteLine(Configurator.GetConstantString("UseLogger"));
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
                if (args[i].Substring(0, 2) == Configurator.GetConstantString("DoubleHyphen"))
                {
                    singleParams.Add(args[i]);
                    continue;
                }
                else if (args[i].Substring(0, 1) == Configurator.GetConstantString("Hyphen"))
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
                    if (keyValuePair[0] == Configurator.GetConstantString("StopwatchCommandLineParameter"))
                    {
                        options.Stopwatch = true;
                    }
                    else if (keyValuePair[0] == Configurator.GetConstantString("LoggerCommandLineParameter"))
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
                    if (keyValuePair[0] == Configurator.GetConstantString("ValidationRuleCommandLineParameter"))
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
                if (doubleParams[i] == Configurator.GetConstantString("ShortValidationRuleCommandLineParameter"))
                {
                    options.Rule = doubleParamsValues[i];
                }
                else if (doubleParams[i] == Configurator.GetConstantString("ShortStorageCommandLineParameter"))
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