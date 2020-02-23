using CommandLine;
using System;
using FileCabinetApp;

namespace FileCabinetGenerator
{
    public class Program
    {
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public static void Main(string[] args)
        {
            Options options = GetCommandLineArguments(args);
            FileCabinetRecord[] records = GenerateRandomRecords(options.StartId, options.RecordsAmount);
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
                    Height = (short)random.Next(1, 300)
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
