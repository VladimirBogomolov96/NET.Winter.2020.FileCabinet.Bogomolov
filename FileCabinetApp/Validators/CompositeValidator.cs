using System;
using System.Collections.Generic;
using System.Linq;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Composition of record validators.
    /// </summary>
    public class CompositeValidator : IRecordValidator
    {
        private readonly List<IRecordValidator> validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidator"/> class.
        /// </summary>
        /// <param name="validators">Validators to compose.</param>
        /// <exception cref="ArgumentNullException">Thrown when validators sequence is null.</exception>
        public CompositeValidator(IEnumerable<IRecordValidator> validators)
        {
            if (validators is null)
            {
                throw new ArgumentNullException(nameof(validators), Configurator.GetConstantString("NullValidatorSequence"));
            }

            this.validators = validators.ToList();
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

            foreach (var validator in this.validators)
            {
                var result = validator.ValidateParameters(record);
                if (result.Item1 is false)
                {
                    Console.WriteLine(result.Item2);
                    return new Tuple<bool, string>(false, result.Item2);
                }
            }

            return new Tuple<bool, string>(true, null);
        }
    }
}
