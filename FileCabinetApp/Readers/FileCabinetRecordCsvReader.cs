using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// Reader of csv records.
    /// </summary>
    public class FileCabinetRecordCsvReader
    {
        private readonly StreamReader fileReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvReader"/> class.
        /// </summary>
        /// <param name="fileReader">File reader.</param>
        public FileCabinetRecordCsvReader(StreamReader fileReader)
        {
            this.fileReader = fileReader;
        }

        /// <summary>
        /// Reads all strings from xml file.
        /// </summary>
        /// <returns>List of records.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();
            this.fileReader.BaseStream.Seek(0, 0);
            this.fileReader.ReadLine();

            while (!this.fileReader.EndOfStream)
            {
                string tempFields = this.fileReader.ReadLine();
                string[] fields = tempFields.Split(',');
                try
                {
                    FileCabinetRecord record = new FileCabinetRecord
                    {
                        Id = int.Parse(fields[0], CultureInfo.InvariantCulture),
                        FirstName = fields[1],
                        LastName = fields[3],
                        DateOfBirth = DateTime.Parse(fields[4], CultureInfo.InvariantCulture),
                        PatronymicLetter = char.Parse(fields[2]),
                        Income = decimal.Parse(fields[6], CultureInfo.InvariantCulture),
                        Height = short.Parse(fields[5].Split('.')[0], CultureInfo.InvariantCulture),
                    };
                    records.Add(record);
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine($"Invalid data in string {tempFields}. Data was skipped.");
                }
                catch (ArgumentException)
                {
                    Console.WriteLine($"Invalid data in string {tempFields}. Data was skipped.");
                }
                catch (FormatException)
                {
                    Console.WriteLine($"Invalid data in string {tempFields}. Data was skipped.");
                }
                catch (OverflowException)
                {
                    Console.WriteLine($"Invalid data in string {tempFields}. Data was skipped.");
                }
            }

            return records;
        }
    }
}
