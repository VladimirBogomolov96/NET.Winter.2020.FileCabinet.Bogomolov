using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Patronymic letter validator.
    /// </summary>
    public class PatronymicValidator : IRecordValidator
    {
        private char from;
        private char to;

        /// <summary>
        /// Initializes a new instance of the <see cref="PatronymicValidator"/> class.
        /// </summary>
        /// <param name="from">Min char.</param>
        /// <param name="to">Max char.</param>
        public PatronymicValidator(char from, char to)
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

            if (record.PatronymicLetter < this.from || record.PatronymicLetter > this.to)
            {
                return new Tuple<bool, string>(false, "Wrong patronymic letter.");
            }

            return new Tuple<bool, string>(true, null);
        }
    }
}
