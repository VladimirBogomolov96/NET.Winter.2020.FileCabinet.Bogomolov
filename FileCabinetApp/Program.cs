using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Xml;
using CommandLine;

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

        private static IFileCabinetService fileCabinetService;
        private static bool isRunning = true;

        private static Func<string, Tuple<bool, string>> firstNameValidator;
        private static Func<string, Tuple<bool, string>> lastNameValidator;
        private static Func<DateTime, Tuple<bool, string>> dateOfBirthValidator;
        private static Func<short, Tuple<bool, string>> heightValidator;
        private static Func<decimal, Tuple<bool, string>> incomeValidator;
        private static Func<char, Tuple<bool, string>> patronymicLetterValidator;

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("export", Export),
            new Tuple<string, Action<string>>("import", Import),
            new Tuple<string, Action<string>>("remove", Remove),
            new Tuple<string, Action<string>>("purge", Purge),
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
            new string[] { "export", "exports current records into file of given format", "The 'export' command exports current records into file of given format." },
            new string[] { "import", "imports records from given file", "The 'import' command imports records from given file." },
            new string[] { "remove", "removes records from service", "The 'remove' command removes records from service." },
            new string[] { "purge", "defragments file", "The 'purge' command defragments file." },
        };

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

        private static void Purge(string parameters)
        {
            if (fileCabinetService is FileCabinetFilesystemService)
            {
                int purgedRecords = fileCabinetService.Purge();
                Console.WriteLine("Data file processing is completed: {0} of {1} records were purged.", purgedRecords, purgedRecords + fileCabinetService.GetStat());
            }
            else
            {
                Console.WriteLine("Purge command can be used only with Filesystem Service.");
            }
        }

        private static void Remove(string parameters)
        {
            if (!int.TryParse(parameters, out int id))
            {
                Console.WriteLine("Enter correct ID.");
                return;
            }

            if (fileCabinetService.Remove(id))
            {
                Console.WriteLine("Record #{0} is removed.", id);
            }
            else
            {
                Console.WriteLine("Record #{0} doesn't exist.", id);
            }
        }

        private static void SetCommandLineSettings(string[] args)
        {
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
                SetCustomService();
            }
            else if (options.Rule.Equals("default", StringComparison.InvariantCultureIgnoreCase))
            {
                SetDefaultService();
            }
            else
            {
                throw new ArgumentException("Wrong command line argument.", nameof(args));
            }
        }

        private static void Import(string parameters)
        {
            string[] parametersArr = parameters.Split(' ', 2);
            if (parametersArr.Length < 2)
            {
                Console.WriteLine("Enter import format and destination file.");
                return;
            }

            const int importTypeIndex = 0;
            const int filePathIndex = 1;

            if (!File.Exists(parametersArr[filePathIndex]))
            {
                Console.WriteLine($"File {parametersArr[filePathIndex]} isn't exist.");
                return;
            }

            FileCabinetServiceSnapshot snapshot = new FileCabinetServiceSnapshot();
            if (parametersArr[importTypeIndex].Equals("csv", StringComparison.OrdinalIgnoreCase))
            {
                using StreamReader fileStream = new StreamReader(parametersArr[filePathIndex]);
                snapshot.LoadFromCsv(fileStream);
                int numberOfImported = fileCabinetService.Restore(snapshot);
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} records were imported from {1}", numberOfImported, parametersArr[filePathIndex]));
            }
            else if (parametersArr[importTypeIndex].Equals("xml", StringComparison.OrdinalIgnoreCase))
            {
                using StreamReader fileStream = new StreamReader(parametersArr[filePathIndex]);
                using XmlReader xmlReader = XmlReader.Create(fileStream);
                snapshot.LoadFromXml(xmlReader);
                int numberOfImported = fileCabinetService.Restore(snapshot);
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} records were imported from {1}", numberOfImported, parametersArr[filePathIndex]));
            }
            else
            {
                Console.WriteLine("Wrong format type.");
            }
        }

        private static void Export(string parameters)
        {
            string[] parametersArr = parameters.Split(' ', 2);
            if (parametersArr.Length < 2)
            {
                Console.WriteLine("Enter export format and destination file.");
                return;
            }

            const int exportTypeIndex = 0;
            const int filePathIndex = 1;

            if (File.Exists(parametersArr[filePathIndex]))
            {
                Console.WriteLine($"File is exist - rewrite {parametersArr[filePathIndex]}? [Y/n]");
                string answer = Console.ReadLine();
                if (answer.Equals("y", StringComparison.OrdinalIgnoreCase))
                {
                    File.Delete(parametersArr[filePathIndex]);
                }
                else
                {
                    return;
                }
            }

            FileCabinetServiceSnapshot snapshot = fileCabinetService.MakeSnapshot();
            if (parametersArr[exportTypeIndex].Equals("csv", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    using StreamWriter streamWriter = new StreamWriter(parametersArr[filePathIndex]);
                    streamWriter.WriteLine("ID,First Name,Patronymic,Last Name,Date Of Birth,Height,Income");
                    snapshot.SaveToCsv(streamWriter);
                    Console.WriteLine($"All records are exported to file {parametersArr[filePathIndex]}");
                }
                catch (DirectoryNotFoundException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else if (parametersArr[exportTypeIndex].Equals("xml", StringComparison.OrdinalIgnoreCase))
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "\t",
                };
                try
                {
                    using XmlWriter xmlWriter = XmlWriter.Create(parametersArr[filePathIndex], settings);
                    snapshot.SaveToXml(xmlWriter);
                    Console.WriteLine($"All records are exported to file {parametersArr[filePathIndex]}");
                }
                catch (DirectoryNotFoundException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Wrong format type.");
            }
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
            Console.Write("First name: ");
            string firstName = ReadInput<string>(ConvertStringToString, firstNameValidator);
            Console.Write("Last name: ");
            string lastName = ReadInput<string>(ConvertStringToString, lastNameValidator);
            Console.Write("Date of birth: ");
            DateTime dateOfBirth = ReadInput<DateTime>(ConvertStringToDateTime, dateOfBirthValidator);
            Console.Write("Height: ");
            short height = ReadInput<short>(ConvertStringToShort, heightValidator);
            Console.Write("Income: ");
            decimal income = ReadInput<decimal>(ConvertStringToDecimal, incomeValidator);
            Console.Write("Patronymic letter: ");
            char patronymicLetter = ReadInput<char>(ConvertStringToChar, patronymicLetterValidator);
            RecordParametersTransfer transfer = new RecordParametersTransfer(firstName, lastName, dateOfBirth, height, income, patronymicLetter);
            try
            {
                int index = fileCabinetService.CreateRecord(transfer);
                Console.WriteLine($"Record #{index} is created.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void List(string parameters)
        {
            ReadOnlyCollection<FileCabinetRecord> records = fileCabinetService.GetRecords();
            if (records.Count is 0)
            {
                Console.WriteLine("There is no stored records.");
            }
            else
            {
                for (int i = 0; i < records.Count; i++)
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
                for (int i = 0; i < fileCabinetService.GetRecords().Count; i++)
                {
                    if (fileCabinetService.GetRecords()[i].Id.ToString(CultureInfo.InvariantCulture) == parameters)
                    {
                        wasFound = true;
                        Console.Write("First name: ");
                        string firstName = ReadInput<string>(ConvertStringToString, firstNameValidator);
                        Console.Write("Last name: ");
                        string lastName = ReadInput<string>(ConvertStringToString, lastNameValidator);
                        Console.Write("Date of birth: ");
                        DateTime dateOfBirth = ReadInput<DateTime>(ConvertStringToDateTime, dateOfBirthValidator);
                        Console.Write("Height: ");
                        short height = ReadInput<short>(ConvertStringToShort, heightValidator);
                        Console.Write("Income: ");
                        decimal income = ReadInput<decimal>(ConvertStringToDecimal, incomeValidator);
                        Console.Write("Patronymic letter: ");
                        char patronymicLetter = ReadInput<char>(ConvertStringToChar, patronymicLetterValidator);
                        RecordParametersTransfer transfer = new RecordParametersTransfer(firstName, lastName, dateOfBirth, height, income, patronymicLetter);
                        fileCabinetService.EditRecord(Convert.ToInt32(parameters, CultureInfo.InvariantCulture), transfer);
                        break;
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

        private static void SetDefaultService()
        {
            fileCabinetService.SetRecordValidator(new DefaultValidator());
            firstNameValidator += FirstNameDefaultValidation;
            lastNameValidator += LastNameDefaultValidation;
            dateOfBirthValidator += DateOfBirthDefaultValidation;
            heightValidator += HeightDefaultValidation;
            incomeValidator += IncomeDefaultValidation;
            patronymicLetterValidator += PatronymicLetterDefaultValidation;
            Console.WriteLine("Using default validation rules.");
        }

        private static void SetCustomService()
        {
            fileCabinetService.SetRecordValidator(new CustomValidator());
            firstNameValidator += FirstNameCustomValidation;
            lastNameValidator += LastNameCustomValidation;
            dateOfBirthValidator += DateOfBirthCustomValidation;
            heightValidator += HeightCustomValidation;
            incomeValidator += IncomeCustomValidation;
            patronymicLetterValidator += PatronymicLetterCustomValidation;
            Console.WriteLine("Using custom validation rules.");
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
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
                        ReadOnlyCollection<FileCabinetRecord> recordsFirstName = fileCabinetService.FindByFirstName(parametersArr[searchValueIndex]);
                        if (recordsFirstName.Count == 0)
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
                        ReadOnlyCollection<FileCabinetRecord> recordsLastName = fileCabinetService.FindByLastName(parametersArr[searchValueIndex]);
                        if (recordsLastName.Count == 0)
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

                        ReadOnlyCollection<FileCabinetRecord> recordsDateOfBirth = fileCabinetService.FindByDateOfbirth(dateOfBirth);
                        if (recordsDateOfBirth.Count == 0)
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

        #region Converters tuples
        private static Tuple<bool, string, DateTime> ConvertStringToDateTime(string input)
        {
            bool isConverted = DateTime.TryParseExact(input, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth);
            if (isConverted)
            {
                return new Tuple<bool, string, DateTime>(isConverted, string.Empty, dateOfBirth);
            }
            else
            {
                return new Tuple<bool, string, DateTime>(isConverted, "DateTime must be in format MM/dd/yyyy", DateTime.MinValue);
            }
        }

        private static Tuple<bool, string, short> ConvertStringToShort(string input)
        {
            bool isConverted = short.TryParse(input, out short height);
            if (isConverted)
            {
                return new Tuple<bool, string, short>(isConverted, string.Empty, height);
            }
            else
            {
                return new Tuple<bool, string, short>(isConverted, "Short must be from -32768 to 32767", short.MinValue);
            }
        }

        private static Tuple<bool, string, decimal> ConvertStringToDecimal(string input)
        {
            bool isConverted = decimal.TryParse(input, out decimal income);
            if (isConverted)
            {
                return new Tuple<bool, string, decimal>(isConverted, string.Empty, income);
            }
            else
            {
                return new Tuple<bool, string, decimal>(isConverted, "Decimal must be from (+/-)1.0*10^-28 to (+/-)7.9228*10^28", decimal.MinValue);
            }
        }

        private static Tuple<bool, string, char> ConvertStringToChar(string input)
        {
            bool isConverted = char.TryParse(input, out char patronymicLetter);
            if (isConverted)
            {
                return new Tuple<bool, string, char>(isConverted, string.Empty, patronymicLetter);
            }
            else
            {
                return new Tuple<bool, string, char>(isConverted, "Char must be a single Unicode symbol", char.MinValue);
            }
        }

        private static Tuple<bool, string, string> ConvertStringToString(string input)
        {
            return new Tuple<bool, string, string>(true, string.Empty, input);
        }
        #endregion

        #region Validation tuples
        private static Tuple<bool, string> FirstNameDefaultValidation(string firstName)
        {
            if (firstName.Length < 2 || firstName.Length > 60)
            {
                return new Tuple<bool, string>(false, "First name length must be from 2 to 60 chars");
            }

            if (firstName.Trim().Length == 0)
            {
                return new Tuple<bool, string>(false, "First name can not contain only whitespaces");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        private static Tuple<bool, string> FirstNameCustomValidation(string firstName)
        {
            if (firstName.Length < 1 || firstName.Length > 40)
            {
                return new Tuple<bool, string>(false, "First name length must be from 1 to 40 chars");
            }

            if (firstName.Trim().Length == 0)
            {
                return new Tuple<bool, string>(false, "First name can not contain only whitespaces");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        private static Tuple<bool, string> LastNameDefaultValidation(string lastName)
        {
            if (lastName.Length < 2 || lastName.Length > 60)
            {
                return new Tuple<bool, string>(false, "Last name length must be from 2 to 60 chars");
            }

            if (lastName.Trim().Length == 0)
            {
                return new Tuple<bool, string>(false, "Last name can not contain only whitespaces");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        private static Tuple<bool, string> LastNameCustomValidation(string lastName)
        {
            if (lastName.Length < 1 || lastName.Length > 40)
            {
                return new Tuple<bool, string>(false, "Last name length must be from 1 to 40 chars");
            }

            if (lastName.Trim().Length == 0)
            {
                return new Tuple<bool, string>(false, "Last name can not contain only whitespaces");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        private static Tuple<bool, string> DateOfBirthDefaultValidation(DateTime dateOfBirth)
        {
            if (dateOfBirth < new DateTime(1950, 1, 1) || dateOfBirth > DateTime.Today)
            {
                return new Tuple<bool, string>(false, "Date of birth must be from 01-Jan-1950 to today");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        private static Tuple<bool, string> DateOfBirthCustomValidation(DateTime dateOfBirth)
        {
            if (dateOfBirth < new DateTime(1900, 1, 1) || dateOfBirth > DateTime.Today)
            {
                return new Tuple<bool, string>(false, "Date of birth must be from 01-Jan-1900 to today");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        private static Tuple<bool, string> HeightDefaultValidation(short height)
        {
            if (height <= 0 || height > 300)
            {
                return new Tuple<bool, string>(false, "Height must be from 1 to 300 cm");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        private static Tuple<bool, string> HeightCustomValidation(short height)
        {
            if (height < 10 || height > 280)
            {
                return new Tuple<bool, string>(false, "Height must be from 10 to 280 cm");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        private static Tuple<bool, string> IncomeDefaultValidation(decimal income)
        {
            if (income < 0)
            {
                return new Tuple<bool, string>(false, "Income must be not negative number");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        private static Tuple<bool, string> IncomeCustomValidation(decimal income)
        {
            if (income < 0 || income > 999999999)
            {
                return new Tuple<bool, string>(false, "Income must be not negative number less than 999999999");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        private static Tuple<bool, string> PatronymicLetterDefaultValidation(char patronymicLetter)
        {
            if (patronymicLetter < 'A' || patronymicLetter > 'Z')
            {
                return new Tuple<bool, string>(false, "Patronymic letter must be a latin letter in uppercase");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        private static Tuple<bool, string> PatronymicLetterCustomValidation(char patronymicLetter)
        {
            if (patronymicLetter < 'A' || patronymicLetter > 'Z')
            {
                return new Tuple<bool, string>(false, "Patronymic letter must be a latin letter in uppercase");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }
        #endregion
    }
}