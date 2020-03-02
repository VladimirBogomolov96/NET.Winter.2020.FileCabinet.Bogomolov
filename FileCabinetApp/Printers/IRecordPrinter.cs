using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Printers
{
    /// <summary>
    /// Provides methods to print records.
    /// </summary>
    public interface IRecordPrinter
    {
        /// <summary>
        /// Prints records.
        /// </summary>
        /// <param name="records">Records to print.</param>
        void Print(IEnumerable<FileCabinetRecord> records);
    }
}
