using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Provides extensions for validator builder.
    /// </summary>
    public static class ValidatorBuilderExtension
    {
        /// <summary>
        /// Creates default composite validator.
        /// </summary>
        /// <param name="validatorBuilder">Builder to create composite validator.</param>
        /// <returns>Default composite validator.</returns>
        /// <exception cref="ArgumentNullException">Thrown when validator builder is null.</exception>
        public static CompositeValidator CreateDefault(this ValidatorBuilder validatorBuilder)
        {
            if (validatorBuilder is null)
            {
                throw new ArgumentNullException(nameof(validatorBuilder), "Validator builder must be not null.");
            }

            return validatorBuilder.
                ValidateDateOfBirth(new DateTime(1950, 1, 1), DateTime.Today).
                ValidateFirstName(2, 60).
                ValidateHeight(0, 300).
                ValidateIncome(0, 1000000).
                ValidateLastName(2, 60).
                ValidatePatronymic('A', 'Z').
                Create();
        }

        /// <summary>
        /// Creates custom composite validator.
        /// </summary>
        /// <param name="validatorBuilder">Builder to create composite validator.</param>
        /// <returns>Custom composite validator.</returns>
        /// <exception cref="ArgumentNullException">Thrown when validator builder is null.</exception>
        public static CompositeValidator CreateCustom(this ValidatorBuilder validatorBuilder)
        {
            if (validatorBuilder is null)
            {
                throw new ArgumentNullException(nameof(validatorBuilder), "Validator builder must be not null.");
            }

            return validatorBuilder.
                ValidateDateOfBirth(new DateTime(1900, 1, 1), DateTime.Today).
                ValidateFirstName(1, 40).
                ValidateHeight(10, 280).
                ValidateIncome(0, 999999999).
                ValidateLastName(1, 40).
                ValidatePatronymic('A', 'Z').
                Create();
        }
    }
}
