using System;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Patronymic letter validator.
    /// </summary>
    public class PatronymicValidator : IRecordValidator
    {
        private readonly char from;
        private readonly char to;

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
                throw new ArgumentNullException(nameof(record), Configurator.GetConstantString("NullRecord"));
            }

            if (record.PatronymicLetter < this.from || record.PatronymicLetter > this.to)
            {
                return new Tuple<bool, string>(false, $"Invalid patronymic letter. Patronymic letter must be from {this.from} to {this.to}.");
            }

            return new Tuple<bool, string>(true, null);
        }
    }
}
