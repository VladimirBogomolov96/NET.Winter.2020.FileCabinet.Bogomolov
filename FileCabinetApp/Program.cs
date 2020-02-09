using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// API class of the program.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Vladimir Bogomolov";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static FileCabinetService fileCabinetService = new FileCabinetService();
        private static bool isRunning = true;

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "returns amount of stored records", "The 'stat' command returns amount of stored records." },
            new string[] { "create", "creates new record with entered data", "The 'create' command creates new record with entered data." },
            new string[] { "list", "returns all stored records", "The 'list' command returns all stored records." },
            new string[] { "edit", "edits existing record", "The 'edit' command edits existing record." },
            new string[] { "find", "finds records by the given condition", "The 'find' command finds records by the given condition." },
        };

        /// <summary>
        /// Point of entrance to program.
        /// </summary>
        /// <param name="args">Command prompt arguments.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            while (true)
            {
                Console.Write("First name: ");
                string firstName = Console.ReadLine();
                Console.Write("Last name: ");
                string lastName = Console.ReadLine();
                Console.Write("Date of birth: ");
                DateTime dateOfBirth;
                try
                {
                    dateOfBirth = DateTime.ParseExact(Console.ReadLine(), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Date of birth must be in the following format: month/day/year");
                    continue;
                }

                Console.Write("Height: ");
                string heightStr = Console.ReadLine();
                if (!short.TryParse(heightStr, out short height))
                {
                    Console.WriteLine("Heigh must be in range of System.Int16 (from -32768 to 32767)");
                    continue;
                }

                Console.Write("Income: ");
                string incomeStr = Console.ReadLine();
                if (!decimal.TryParse(incomeStr, out decimal income))
                {
                    Console.WriteLine("Income must be a decimal number from (+/-)1.0*10^(-28) to (+/-)7.9228*10^28");
                    continue;
                }

                Console.Write("Patronymic letter: ");
                string patronymicLetterStr = Console.ReadLine();
                if (!char.TryParse(patronymicLetterStr, out char patronymicLetter))
                {
                    Console.WriteLine("Enter only one symbol.");
                    continue;
                }

                try
                {
                    int index = fileCabinetService.CreateRecord(firstName, lastName, dateOfBirth, height, income, patronymicLetter);
                    Console.WriteLine($"Record #{index} is created.");
                    break;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }
            }
        }

        private static void List(string parameters)
        {
            FileCabinetRecord[] records = fileCabinetService.GetRecords();
            if (records.Length is 0)
            {
                Console.WriteLine("There is no stored records.");
            }
            else
            {
                for (int i = 0; i < records.Length; i++)
                {
                    Console.WriteLine(
                        "#{0}, {1}, {2}., {3}, {4}, {5} cm, {6}$",
                        records[i].Id,
                        records[i].FirstName,
                        records[i].PatronymicLetter,
                        records[i].LastName,
                        records[i].DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture),
                        records[i].Height,
                        records[i].Income);
                }
            }
        }

        private static void Edit(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                bool wasFound = false;
                for (int i = 0; i < fileCabinetService.GetRecords().Length; i++)
                {
                    if (fileCabinetService.GetRecords()[i].Id.ToString(CultureInfo.InvariantCulture) == parameters)
                    {
                        wasFound = true;
                        while (true)
                        {
                            Console.Write("First name: ");
                            string firstName = Console.ReadLine();
                            Console.Write("Last name: ");
                            string lastName = Console.ReadLine();
                            Console.Write("Date of birth: ");
                            DateTime dateOfBirth;
                            try
                            {
                                dateOfBirth = DateTime.ParseExact(Console.ReadLine(), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                            }
                            catch (FormatException)
                            {
                                Console.WriteLine("Date of birth must be in the following format: month/day/year");
                                continue;
                            }

                            Console.Write("Height: ");
                            string heightStr = Console.ReadLine();
                            if (!short.TryParse(heightStr, out short height))
                            {
                                Console.WriteLine("Heigh must be in range of System.Int16 (from -32768 to 32767)");
                                continue;
                            }

                            Console.Write("Income: ");
                            string incomeStr = Console.ReadLine();
                            if (!decimal.TryParse(incomeStr, out decimal income))
                            {
                                Console.WriteLine("Income must be a decimal number from (+/-)1.0*10^(-28) to (+/-)7.9228*10^28");
                                continue;
                            }

                            Console.Write("Patronymic letter: ");
                            string patronymicLetterStr = Console.ReadLine();
                            if (!char.TryParse(patronymicLetterStr, out char patronymicLetter))
                            {
                                Console.WriteLine("Enter only one symbol.");
                                continue;
                            }

                            fileCabinetService.EditRecord(Convert.ToInt32(parameters, CultureInfo.InvariantCulture), firstName, lastName, dateOfBirth, height, income, patronymicLetter);
                            break;
                        }
                    }
                }

                if (!wasFound)
                {
                    Console.WriteLine($"#{parameters} record is not found.");
                }
            }
            else
            {
                Console.WriteLine("Input record ID to edit.");
            }
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }

        private static void Find(string parameters)
        {
            while (true)
            {
                string[] parametersArr = parameters.Split(' ', 2);
                if (parametersArr.Length < 2)
                {
                    Console.WriteLine("Enter property parameter and value to search.");
                    break;
                }

                const int propertyIndex = 0;
                const int searchValueIndex = 1;
                parametersArr[propertyIndex] = parametersArr[propertyIndex].ToUpperInvariant();
                switch (parametersArr[propertyIndex])
                {
                    case "FIRSTNAME":
                        FileCabinetRecord[] recordsFirstName = fileCabinetService.FindByFirstName(parametersArr[searchValueIndex]);
                        if (recordsFirstName.Length == 0)
                        {
                            Console.WriteLine("Such records don't exist.");
                            break;
                        }

                        foreach (FileCabinetRecord record in recordsFirstName)
                        {
                            Console.WriteLine(
                            "#{0}, {1}, {2}., {3}, {4}, {5} cm, {6}$",
                            record.Id,
                            record.FirstName,
                            record.PatronymicLetter,
                            record.LastName,
                            record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture),
                            record.Height,
                            record.Income);
                        }

                        break;
                    case "LASTNAME":
                        FileCabinetRecord[] recordsLastName = fileCabinetService.FindByLastName(parametersArr[searchValueIndex]);
                        if (recordsLastName.Length == 0)
                        {
                            Console.WriteLine("Such records don't exist.");
                            break;
                        }

                        foreach (FileCabinetRecord record in recordsLastName)
                        {
                            Console.WriteLine(
                            "#{0}, {1}, {2}., {3}, {4}, {5} cm, {6}$",
                            record.Id,
                            record.FirstName,
                            record.PatronymicLetter,
                            record.LastName,
                            record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture),
                            record.Height,
                            record.Income);
                        }

                        break;
                    case "DATEOFBIRTH":
                        DateTime dateOfBirth;
                        try
                        {
                            dateOfBirth = DateTime.ParseExact(parametersArr[searchValueIndex], "MM/dd/yyyy", CultureInfo.InvariantCulture);
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Date of birth must be in the following format: month/day/year");
                            break;
                        }

                        FileCabinetRecord[] recordsDateOfBirth = fileCabinetService.FindByDateOfbirth(dateOfBirth);
                        if (recordsDateOfBirth.Length == 0)
                        {
                            Console.WriteLine("Such records don't exist.");
                            break;
                        }

                        foreach (FileCabinetRecord record in recordsDateOfBirth)
                        {
                            Console.WriteLine(
                            "#{0}, {1}, {2}., {3}, {4}, {5} cm, {6}$",
                            record.Id,
                            record.FirstName,
                            record.PatronymicLetter,
                            record.LastName,
                            record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture),
                            record.Height,
                            record.Income);
                        }

                        break;
                    default:
                        Console.WriteLine("Wrong property parameter.");
                        break;
                }

                break;
            }
        }
    }
}