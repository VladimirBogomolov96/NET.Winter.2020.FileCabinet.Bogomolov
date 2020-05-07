using System;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Income validator.
    /// </summary>
    public class IncomeValidator : IRecordValidator
    {
        private readonly decimal from;
        private readonly decimal to;

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
                throw new ArgumentNullException(nameof(record), Configurator.GetConstantString("NullRecord"));
            }

            if (record.Income < this.from || record.Income > this.to)
            {
                return new Tuple<bool, string>(false, $"Invalid income. Income must be from {this.from} to {this.to}.");
            }

            return new Tuple<bool, string>(true, null);
        }
    }
}
