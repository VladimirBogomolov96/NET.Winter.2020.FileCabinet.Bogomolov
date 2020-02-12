using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides methods to save records into csv or xml.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private readonly FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">Records to save.</param>
        public FileCabinetServiceSnapshot(FileCabinetRecord[] records)
        {
            this.records = records;
        }

        /// <summary>
        /// Saves records to csv file.
        /// </summary>
        /// <param name="writer">Streamwriter to save records.</param>
        public void SaveToCsv(StreamWriter writer)
        {
            using FileCabinetRecordCsvWriter csvWriter = new FileCabinetRecordCsvWriter(writer);
            for (int i = 0; i < this.records.Length; i++)
            {
                csvWriter.Write(this.records[i]);
            }
        }
    }
}
