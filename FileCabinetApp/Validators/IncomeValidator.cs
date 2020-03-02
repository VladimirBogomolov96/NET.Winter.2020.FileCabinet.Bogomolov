using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Income validator.
    /// </summary>
    public class IncomeValidator : IRecordValidator
    {
        private decimal from;
        private decimal to;

        /// <summary>
        /// Initializes a new instance of the <see cref="IncomeValidator"/> class.
        /// </summary>
        /// <param name="from">Min income.</param>
        /// <param name="to">Max income.</param>
        public IncomeValidator(decimal from, decimal to)
        {
            this.from = from;
            this.to = to;
        }

        /// <summary>
        /// Validate given record.
        /// </summary>
        /// <param name="record">Record to validate.</param>
        /// <returns>Whether validation was succesful and reason of fail.</returns>
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        public Tuple<bool, string> ValidateParameters(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), "Record must be not null.");
            }

            if (record.Income < this.from || record.Income > this.to)
            {
                return new Tuple<bool, string>(false, "Wrong income.");
            }

            return new Tuple<bool, string>(true, null);
        }
    }
}
