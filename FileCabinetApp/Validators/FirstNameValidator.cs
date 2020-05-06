using System;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// First name validator.
    /// </summary>
    public class FirstNameValidator : IRecordValidator
    {
        private readonly int minLength;
        private readonly int maxLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirstNameValidator"/> class.
        /// </summary>
        /// <param name="minLength">Min length of first name.</param>
        /// <param name="maxLength">Max length of first name.</param>
        public FirstNameValidator(int minLength, int maxLength)
        {
            this.minLength = minLength;
            this.maxLength = maxLength;
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

            if (record.FirstName is null || record.FirstName.Length < this.minLength || record.FirstName.Length > this.maxLength || record.FirstName.Trim().Length is 0)
            {
                return new Tuple<bool, string>(false, $"Invalid first name. First name length must be from {this.minLength} to {this.maxLength}.");
            }

            return new Tuple<bool, string>(true, null);
        }
    }
}
