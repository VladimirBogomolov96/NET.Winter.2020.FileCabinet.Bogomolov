using System;
using System.Globalization;

namespace FileCabinetApp
{
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
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "returns amount of stored records", "The 'stat' command returns amount of stored records." },
            new string[] { "create", "creates new record with entered data", "The 'create' command creates new record with entered data." },
            new string[] { "list", "returns all stored records", "The 'list' command returns all stored records." },
        };

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
                DateTime dateOfBirth = DateTime.MinValue;
                try
                {
                    dateOfBirth = DateTime.ParseExact(Console.ReadLine(), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Date of birth must be in the following format: month/day/year");
                    break;
                }

                Console.Write("Height: ");
                string heightStr = Console.ReadLine();
                short height;
                if (!short.TryParse(heightStr, out height) || height < 0)
                {
                    Console.WriteLine("Heigh must be in range of 0 and 32767");
                    break;
                }

                Console.Write("Income: ");
                string incomeStr = Console.ReadLine();
                decimal income;
                if (!decimal.TryParse(incomeStr, out income))
                {
                    Console.WriteLine("Income must be a decimal number.");
                    break;
                }

                Console.Write("Favourite symbol: ");
                string favouriteSymbolStr = Console.ReadLine();
                char favouriteSymbol;
                if (!char.TryParse(favouriteSymbolStr, out favouriteSymbol))
                {
                    Console.WriteLine("Enter only one symbol.");
                    break;
                }

                int index = fileCabinetService.CreateRecord(firstName, lastName, dateOfBirth, height, income, favouriteSymbol);
                Console.WriteLine($"Record #{index} is created.");
                break;
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
                        "#{0}, {1}, {2}, {3}, {4} cm, {5}$, {6}",
                        records[i].Id,
                        records[i].FirstName,
                        records[i].LastName,
                        records[i].DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture),
                        records[i].Height,
                        records[i].Income,
                        records[i].FavouriteSymbol);
                }
            }
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }
    }
}