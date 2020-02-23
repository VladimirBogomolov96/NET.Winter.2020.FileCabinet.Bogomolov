using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using CommandLine;
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

                    Console.WriteLine($"All records are exported to file {options.OutputFileName}");
                }
                catch (DirectoryNotFoundException ex)
                {
                    Console.WriteLine(ex.Message);
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

                    Console.WriteLine($"All records are exported to file {options.OutputFileName}");
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

        private static Options GetCommandLineArguments(string[] args)
        {
            Options options = new Options();
            var result = Parser.Default.ParseArguments<Options>(args).WithParsed(parsed => options = parsed);
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
                FileCabinetRecord newRecord = new FileCabinetRecord
                {
                    Id = start++,
                    FirstName = GetRandomString(Alphabet, random.Next(2, 60), random),
                    LastName = GetRandomString(Alphabet, random.Next(2, 60), random),
                    DateOfBirth = GetRandomDate(new DateTime(1950, 1, 1), random),
                    PatronymicLetter = (char)random.Next((int)'A', (int)'Z'),
                    Income = random.Next(),
                    Height = (short)random.Next(1, 300),
                };
                records[i] = newRecord;
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
