using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp.Printers
{
    /// <summary>
    /// Default printer for records.
    /// </summary>
    public class DefaultPrinter : IRecordPrinter
    {
        /// <summary>
        /// Prints records.
        /// </summary>
        /// <param name="records">Records to print.</param>
        /// <exception cref="ArgumentNullException">Thrown when given records are null.</exception>
        public void Print(IEnumerable<FileCabinetRecord> records)
        {
            if (records is null)
            {
                throw new ArgumentNullException(nameof(records), Configurator.GetConstantString("NullRecordsSequence"));
            }

            foreach (var record in records)
            {
                if (record is null)
                {
                    continue;
                }

                Console.WriteLine(
                        Configurator.GetConstantString("PrintPatthern"),
                        record.Id,
                        record.FirstName,
                        record.PatronymicLetter,
                        record.LastName,
                        record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture),
                        record.Height,
                        record.Income);
            }
        }
    }
}
