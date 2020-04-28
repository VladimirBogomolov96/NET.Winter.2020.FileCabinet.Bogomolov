using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using FileCabinetApp;

namespace FileCabinetGenerator
{
    /// <summary>
    /// API class of the program.
    /// </summary>
    public static class Program
    {
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// Point of entrance to program.
        /// </summary>
        /// <param name="args">Command prompt arguments.</param>
        public static void Main(string[] args)
        {
            Options options = GetCommandLineArguments(args);
            FileCabinetRecord[] records = GenerateRandomRecords(options.StartId, options.RecordsAmount);
            Export(options, records);
        }

        private static void Export(Options options, IEnumerable<FileCabinetRecord> records)
        {
            if (File.Exists(options.OutputFileName))
            {
                Console.WriteLine($"File is exist - rewrite {options.OutputFileName}? [Y/n]");
                string answer = Console.ReadLine();
                if (answer.Equals("y", StringComparison.OrdinalIgnoreCase))
                {
                    File.Delete(options.OutputFileName);
                }
                else
                {
                    return;
                }
            }

            if (options.OutputType.Equals("csv", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    using StreamWriter streamWriter = new StreamWriter(options.OutputFileName);
                    streamWriter.WriteLine("ID,First Name,Patronymic,Last Name,Date Of Birth,Height,Income");
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
                }
            }
            else if (options.OutputType.Equals("xml", StringComparison.OrdinalIgnoreCase))
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
                }
            }
            else
            {
                Console.WriteLine($"Wrong format type {options.OutputType}.");
            }
        }

        private static Options GetCommandLineArguments(string[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args), "Args must be not null");
            }

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
                if (keyValuePair.Length == 2)
                {
                    if (keyValuePair[0] == "--output-type")
                    {
                        options.OutputType = keyValuePair[1];
                    }
                    else if (keyValuePair[0] == "--output")
                    {
                        options.OutputFileName = keyValuePair[1];
                    }
                    else if (keyValuePair[0] == "--records-amount")
                    {
                        if (!int.TryParse(keyValuePair[1], out int amount))
                        {
                            Console.WriteLine($"Invalid command line parameter '{param}'.");
                            Environment.Exit(-1);
                        }

                        options.RecordsAmount = amount;
                    }
                    else if (keyValuePair[0] == "--start-id")
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
                if (doubleParams[i] == "-t")
                {
                    options.OutputType = doubleParamsValues[i];
                }
                else if (doubleParams[i] == "-o")
                {
                    options.OutputFileName = doubleParamsValues[i];
                }
                else if (doubleParams[i] == "-a")
                {
                    if (!int.TryParse(doubleParamsValues[i], out int amount))
                    {
                        Console.WriteLine($"Invalid command line parameter value '{doubleParamsValues[i]}'.");
                        Environment.Exit(-1);
                    }

                    options.RecordsAmount = amount;
                }
                else if (doubleParams[i] == "-i")
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
                throw new ArgumentOutOfRangeException(nameof(start), "Index must be more than 0.");
            }

            FileCabinetRecord[] records = new FileCabinetRecord[amount];
            Random random = new Random();
            for (int i = 0; i < amount; i++)
            {
                records[i] = new FileCabinetRecord
                {
                    Id = start++,
                    FirstName = GetRandomString(Alphabet, random.Next(2, 60), random),
                    LastName = GetRandomString(Alphabet, random.Next(2, 60), random),
                    DateOfBirth = GetRandomDate(new DateTime(1950, 1, 1), random),
                    PatronymicLetter = (char)random.Next((int)'A', (int)'Z'),
                    Income = random.Next(0, 1000000),
                    Height = (short)random.Next(1, 300),
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

        private static DateTime GetRandomDate(DateTime minValue, Random random)
        {
            int range = (DateTime.Today - minValue).Days;
            return minValue.AddDays(random.Next(range));
        }
    }
}
