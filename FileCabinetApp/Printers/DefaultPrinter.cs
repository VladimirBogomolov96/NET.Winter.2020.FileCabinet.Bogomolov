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
        public void Print(IEnumerable<FileCabinetRecord> records)
        {
            if (records is null)
            {
                throw new ArgumentNullException(nameof(records), "Sequence of records must be not null.");
            }

            foreach (var record in records)
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
        }
    }
}
