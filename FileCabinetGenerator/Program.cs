using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using FileCabinetApp;
using Microsoft.Extensions.Configuration;

namespace FileCabinetGenerator
{
    /// <summary>
    /// API class of the program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Point of entrance to program.
        /// </summary>
        /// <param name="args">Command prompt arguments.</param>
        public static void Main(string[] args)
        {
            Options options = GetCommandLineArguments(args);
            FileCabinetRecord[] records = null;
            try
            {
                records = GenerateRandomRecords(options.StartId, options.RecordsAmount);
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine(Configurator.GetConstantString("IndexLess1"));
                Console.WriteLine(Configurator.GetConstantString("ClosingProgram"));
                Environment.Exit(-1);
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine(Configurator.GetConstantString("InvalidValidationFile"));
                Console.WriteLine(Configurator.GetConstantString("ClosingProgram"));
                Environment.Exit(-1);
            }
            catch (FormatException)
            {
                Console.WriteLine(Configurator.GetConstantString("InvalidValidationFile"));
                Console.WriteLine(Configurator.GetConstantString("ClosingProgram"));
                Environment.Exit(-1);
            }
            catch (OverflowException)
            {
                Console.WriteLine(Configurator.GetConstantString("ValidationOutOfRange"));
                Console.WriteLine(Configurator.GetConstantString("ClosingProgram"));
                Environment.Exit(-1);
            }

            Export(options, records);
        }

        private static void Export(Options options, IEnumerable<FileCabinetRecord> records)
        {
            if (File.Exists(options.OutputFileName))
            {
                Console.WriteLine($"File is exist - rewrite {options.OutputFileName}? [Y/n]");
                string answer = Console.ReadLine();
                if (answer.Equals(Configurator.GetConstantString("PositiveAnswer"), StringComparison.OrdinalIgnoreCase))
                {
                    File.Delete(options.OutputFileName);
                }
                else
                {
                    return;
                }
            }

            if (options.OutputType.Equals(Configurator.GetConstantString("Csv"), StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    using StreamWriter streamWriter = new StreamWriter(options.OutputFileName);
                    streamWriter.WriteLine(Configurator.GetConstantString("CsvHeader"));
                    using FileCabinetRecordCsvWriter csvWriter = new FileCabinetRecordCsvWriter(streamWriter);
                    foreach (FileCabinetRecord record in records)
                    {
                        csvWriter.Write(record);
                    }

                    Console.WriteLine($"{records.Count()} records are exported to file {options.OutputFileName}");
                }
                catch (DirectoryNotFoundException)
                {
                    Console.WriteLine($"Can't find directory {options.OutputFileName}.");
                    Console.WriteLine(Configurator.GetConstantString("ClosingProgram"));
                }
            }
            else if (options.OutputType.Equals(Configurator.GetConstantString("Xml"), StringComparison.OrdinalIgnoreCase))
            {
                FileCabinetRecordXml[] xmlRecords = new FileCabinetRecordXml[records.Count()];
                int counter = 0;
                foreach (FileCabinetRecord record in records)
                {
                    xmlRecords[counter] = new FileCabinetRecordXml(record);
                    counter++;
                }

                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                };
                try
                {
                    using (var fileWriter = XmlWriter.Create(options.OutputFileName, settings))
                    {
                        XmlWriterToGenerator xmlWriter = new XmlWriterToGenerator(fileWriter, xmlRecords);
                        xmlWriter.Write();
                    }

                    Console.WriteLine($"{records.Count()} records are exported to file {options.OutputFileName}");
                }
                catch (DirectoryNotFoundException)
                {
                    Console.WriteLine($"Can't find directory {options.OutputFileName}.");
                    Console.WriteLine(Configurator.GetConstantString("ClosingProgram"));
                }
            }
            else
            {
                Console.WriteLine($"Wrong format type {options.OutputType}.");
                Console.WriteLine(Configurator.GetConstantString("ClosingProgram"));
            }
        }

        private static Options GetCommandLineArguments(string[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args), Configurator.GetConstantString("ArgsIsNull"));
            }

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
                if (keyValuePair.Length == 2)
                {
                    if (keyValuePair[0] == Configurator.GetConstantString("OutputTypeCommandLineParameter"))
                    {
                        options.OutputType = keyValuePair[1];
                    }
                    else if (keyValuePair[0] == Configurator.GetConstantString("OutputPathCommandLineParameter"))
                    {
                        options.OutputFileName = keyValuePair[1];
                    }
                    else if (keyValuePair[0] == Configurator.GetConstantString("RecordsAmountCommandLineParameter"))
                    {
                        if (!int.TryParse(keyValuePair[1], out int amount))
                        {
                            Console.WriteLine($"Invalid command line parameter '{param}'.");
                            Environment.Exit(-1);
                        }

                        options.RecordsAmount = amount;
                    }
                    else if (keyValuePair[0] == Configurator.GetConstantString("StartIdCommandLineParameter"))
                    {
                        if (!int.TryParse(keyValuePair[1], out int startId))
                        {
                            Console.WriteLine($"Invalid command line parameter '{param}'.");
                            Environment.Exit(-1);
                        }

                        options.StartId = startId;
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
                if (doubleParams[i] == Configurator.GetConstantString("ShortOutputTypeCommandLineParameter"))
                {
                    options.OutputType = doubleParamsValues[i];
                }
                else if (doubleParams[i] == Configurator.GetConstantString("ShortOutputPathCommandLineParameter"))
                {
                    options.OutputFileName = doubleParamsValues[i];
                }
                else if (doubleParams[i] == Configurator.GetConstantString("ShortRecordsAmountCommandLineParameter"))
                {
                    if (!int.TryParse(doubleParamsValues[i], out int amount))
                    {
                        Console.WriteLine($"Invalid command line parameter value '{doubleParamsValues[i]}'.");
                        Environment.Exit(-1);
                    }

                    options.RecordsAmount = amount;
                }
                else if (doubleParams[i] == Configurator.GetConstantString("ShortStartIdCommandLineParameter"))
                {
                    if (!int.TryParse(doubleParamsValues[i], out int startId))
                    {
                        Console.WriteLine($"Invalid command line parameter value '{doubleParamsValues[i]}'.");
                        Environment.Exit(-1);
                    }

                    options.StartId = startId;
                }
                else
                {
                    Console.WriteLine($"Invalid command line parameter '{doubleParams[i]}'.");
                    Environment.Exit(-1);
                }
            }

            return options;
        }

        private static FileCabinetRecord[] GenerateRandomRecords(int start, int amount)
        {
            if (start < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(start), Configurator.GetConstantString("IndexLess1"));
            }

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

            int minFirstNameLength = int.Parse(validationRules.GetSection("default").GetSection("firstName").GetSection("minLength").Value, CultureInfo.InvariantCulture);
            int maxFirstNameLength = int.Parse(validationRules.GetSection("default").GetSection("firstName").GetSection("maxLength").Value, CultureInfo.InvariantCulture);
            int minLastNameLength = int.Parse(validationRules.GetSection("default").GetSection("lastName").GetSection("minLength").Value, CultureInfo.InvariantCulture);
            int maxLastNameLength = int.Parse(validationRules.GetSection("default").GetSection("lastName").GetSection("maxLength").Value, CultureInfo.InvariantCulture);
            DateTime fromDateOfBirth = DateTime.ParseExact(validationRules.GetSection("default").GetSection("dateOfBirth").GetSection("from").Value, Configurator.GetConstantString("DateFormatDM"), CultureInfo.InvariantCulture);
            DateTime toDateOfBirth = DateTime.ParseExact(validationRules.GetSection("default").GetSection("dateOfBirth").GetSection("to").Value, Configurator.GetConstantString("DateFormatDM"), CultureInfo.InvariantCulture);
            char minPatronymicLetter = char.Parse(validationRules.GetSection("default").GetSection("patronymicLetter").GetSection("from").Value);
            char maxPatronymicLetter = char.Parse(validationRules.GetSection("default").GetSection("patronymicLetter").GetSection("to").Value);
            decimal minIncome = decimal.Parse(validationRules.GetSection("default").GetSection("income").GetSection("from").Value, CultureInfo.InvariantCulture);
            decimal maxIncome = decimal.Parse(validationRules.GetSection("default").GetSection("income").GetSection("to").Value, CultureInfo.InvariantCulture);
            short minHeight = short.Parse(validationRules.GetSection("default").GetSection("height").GetSection("min").Value, CultureInfo.InvariantCulture);
            short maxHeight = short.Parse(validationRules.GetSection("default").GetSection("height").GetSection("max").Value, CultureInfo.InvariantCulture);
            string alphabet = Configurator.GetConstantString("Alphabet");

            FileCabinetRecord[] records = new FileCabinetRecord[amount];
            Random random = new Random();
            for (int i = 0; i < amount; i++)
            {
                records[i] = new FileCabinetRecord
                {
                    Id = start++,
                    FirstName = GetRandomString(alphabet, random.Next(minFirstNameLength, maxFirstNameLength), random),
                    LastName = GetRandomString(alphabet, random.Next(minLastNameLength, maxLastNameLength), random),
                    DateOfBirth = GetRandomDate(fromDateOfBirth, toDateOfBirth, random),
                    PatronymicLetter = (char)random.Next((int)minPatronymicLetter, (int)maxPatronymicLetter),
                    Income = random.Next((int)minIncome, (int)maxIncome - 1) + (((decimal)random.Next(0, 100)) / 100),
                    Height = (short)random.Next(minHeight, maxHeight),
                };
            }

            return records;
        }

        private static string GetRandomString(string charsArr, int length, Random random)
        {
            char[] result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = charsArr[random.Next(charsArr.Length - 1)];
            }

            return new string(result);
        }

        private static DateTime GetRandomDate(DateTime from, DateTime to, Random random)
        {
            int range = (to - from).Days;
            return from.AddDays(random.Next(range));
        }
    }
}
