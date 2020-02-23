using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace FileCabinetApp
{
    public class FileCabinetRecordCsvReader
    {
        private StreamReader fileReader;

        public FileCabinetRecordCsvReader(StreamReader fileReader)
        {
            this.fileReader = fileReader;
        }

        public IList<FileCabinetRecord> ReadAll()
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();
            this.fileReader.BaseStream.Seek(0, 0);
            string names = this.fileReader.ReadLine();

            while (!this.fileReader.EndOfStream)
            {
                string[] fields = this.fileReader.ReadLine().Split(',');
                FileCabinetRecord record = new FileCabinetRecord();
                record.Id = int.Parse(fields[0], CultureInfo.InvariantCulture);
                record.FirstName = fields[1];
                record.LastName = fields[3];
                record.DateOfBirth = DateTime.Parse(fields[4], CultureInfo.InvariantCulture);
                record.PatronymicLetter = char.Parse(fields[2]);
                record.Income = decimal.Parse(fields[6], CultureInfo.InvariantCulture);
                record.Height = short.Parse(fields[5].Split('.')[0], CultureInfo.InvariantCulture);
                records.Add(record);
            }

            return records;
        }
    }
}
