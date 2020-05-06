using System;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Height validator.
    /// </summary>
    public class HeightValidator : IRecordValidator
    {
        private readonly short minHeight;
        private readonly short maxHeight;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeightValidator"/> class.
        /// </summary>
        /// <param name="minHeight">Min height.</param>
        /// <param name="maxHeight">Max height.</param>
        public HeightValidator(short minHeight, short maxHeight)
        {
            this.minHeight = minHeight;
            this.maxHeight = maxHeight;
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

            if (record.Height < this.minHeight || record.Height > this.maxHeight)
            {
                return new Tuple<bool, string>(false, $"Invalid height. Height must be form {this.minHeight} to {this.maxHeight}.");
            }

            return new Tuple<bool, string>(true, null);
        }
    }
}
