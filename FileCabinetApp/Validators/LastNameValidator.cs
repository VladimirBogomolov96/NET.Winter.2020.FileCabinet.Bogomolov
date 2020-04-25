using System;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Last name validator.
    /// </summary>
    public class LastNameValidator : IRecordValidator
    {
        private readonly int minLength;
        private readonly int maxLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="LastNameValidator"/> class.
        /// </summary>
        /// <param name="minLength">Min length of last name.</param>
        /// <param name="maxLength">Max length of last name.</param>
        public LastNameValidator(int minLength, int maxLength)
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
                throw new ArgumentNullException(nameof(record), "Record must be not null.");
            }

            if (record.LastName is null || record.LastName.Length < this.minLength || record.LastName.Length > this.maxLength || record.LastName.Trim().Length is 0)
            {
                return new Tuple<bool, string>(false, "Wrong last name.");
            }

            return new Tuple<bool, string>(true, null);
        }
    }
}
