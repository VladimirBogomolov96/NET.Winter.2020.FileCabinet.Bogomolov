using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Validator builder.
    /// </summary>
    public class ValidatorBuilder
    {
        private List<IRecordValidator> validators = new List<IRecordValidator>();

        /// <summary>
        /// Adds first name validation.
        /// </summary>
        /// <param name="minLength">Min length of first name.</param>
        /// <param name="maxLength">Max length of first name.</param>
        /// <returns>Validator builder.</returns>
        public ValidatorBuilder ValidateFirstName(int minLength, int maxLength)
        {
            this.validators.Add(new FirstNameValidator(minLength, maxLength));
            return this;
        }

        /// <summary>
        /// Adds last name validation.
        /// </summary>
        /// <param name="minLength">Min length of last name.</param>
        /// <param name="maxLength">Max length of last name.</param>
        /// <returns>Validator builder.</returns>
        public ValidatorBuilder ValidateLastName(int minLength, int maxLength)
        {
            this.validators.Add(new LastNameValidator(minLength, maxLength));
            return this;
        }

        /// <summary>
        /// Adds date of birth validation.
        /// </summary>
        /// <param name="from">Min value of date of birth.</param>
        /// <param name="to">Max value of date of birth.</param>
        /// <returns>Validator builder.</returns>
        public ValidatorBuilder ValidateDateOfBirth(DateTime from, DateTime to)
        {
            this.validators.Add(new DateOfBirthValidator(from, to));
            return this;
        }

        /// <summary>
        /// Adds patronymic letter validation.
        /// </summary>
        /// <param name="from">Min char.</param>
        /// <param name="to">Max char.</param>
        /// <returns>Validator builder.</returns>
        public ValidatorBuilder ValidatePatronymic(char from, char to)
        {
            this.validators.Add(new PatronymicValidator(from, to));
            return this;
        }

        /// <summary>
        /// Adds income validation.
        /// </summary>
        /// <param name="from">Min income.</param>
        /// <param name="to">Max income.</param>
        /// <returns>Validator builder.</returns>
        public ValidatorBuilder ValidateIncome(decimal from, decimal to)
        {
            this.validators.Add(new IncomeValidator(from, to));
            return this;
        }

        /// <summary>
        /// Adds height validation.
        /// </summary>
        /// <param name="min">Min height.</param>
        /// <param name="max">Max height.</param>
        /// <returns>Validator builder.</returns>
        public ValidatorBuilder ValidateHeight(short min, short max)
        {
            this.validators.Add(new HeightValidator(min, max));
            return this;
        }

        /// <summary>
        /// Creates composite validator.
        /// </summary>
        /// <returns>Composite validator.</returns>
        public CompositeValidator Create()
        {
            return new CompositeValidator(this.validators);
        }
    }
}
