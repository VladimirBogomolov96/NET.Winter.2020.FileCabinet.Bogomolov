using System;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Date of birth validator.
    /// </summary>
    public class DateOfBirthValidator : IRecordValidator
    {
        private readonly DateTime from;
        private readonly DateTime to;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateOfBirthValidator"/> class.
        /// </summary>
        /// <param name="from">Min value of date of birth.</param>
        /// <param name="to">Max value of date of birth.</param>
        public DateOfBirthValidator(DateTime from, DateTime to)
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

            if (record.DateOfBirth < this.from || record.DateOfBirth > this.to)
            {
                return new Tuple<bool, string>(false, $"Invalid date of birth. Date of birth must be from {this.from.Month}/{this.from.Day}/{this.from.Year} to {this.to.Month}/{this.to.Day}/{this.to.Year}.");
            }

            return new Tuple<bool, string>(true, null);
        }
    }
}
