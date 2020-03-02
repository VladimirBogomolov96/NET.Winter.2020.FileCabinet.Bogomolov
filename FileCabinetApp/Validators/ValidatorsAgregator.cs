using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Agregates validators.
    /// </summary>
    public static class ValidatorsAgregator
    {
        /// <summary>
        /// First name validator.
        /// </summary>
        public static Func<string, Tuple<bool, string>> firstNameValidator;

        /// <summary>
        /// Last name validator.
        /// </summary>
        public static Func<string, Tuple<bool, string>> lastNameValidator;

        /// <summary>
        /// Date of birth validator.
        /// </summary>
        public static Func<DateTime, Tuple<bool, string>> dateOfBirthValidator;

        /// <summary>
        /// Height validator.
        /// </summary>
        public static Func<short, Tuple<bool, string>> heightValidator;

        /// <summary>
        /// Income validator.
        /// </summary>
        public static Func<decimal, Tuple<bool, string>> incomeValidator;

        /// <summary>
        /// Patronymic letter validator.
        /// </summary>
        public static Func<char, Tuple<bool, string>> patronymicLetterValidator;

        /// <summary>
        /// Sets default validators.
        /// </summary>
        public static void SetDefaultValidators()
        {
            firstNameValidator += FirstNameDefaultValidation;
            lastNameValidator += LastNameDefaultValidation;
            dateOfBirthValidator += DateOfBirthDefaultValidation;
            heightValidator += HeightDefaultValidation;
            incomeValidator += IncomeDefaultValidation;
            patronymicLetterValidator += PatronymicLetterDefaultValidation;
        }

        /// <summary>
        /// Sets custom validators.
        /// </summary>
        public static void SetCustomValidators()
        {
            firstNameValidator += FirstNameCustomValidation;
            lastNameValidator += LastNameCustomValidation;
            dateOfBirthValidator += DateOfBirthCustomValidation;
            heightValidator += HeightCustomValidation;
            incomeValidator += IncomeCustomValidation;
            patronymicLetterValidator += PatronymicLetterCustomValidation;
        }

        /// <summary>
        /// Validates first name.
        /// </summary>
        /// <param name="firstName">First name to validate.</param>
        /// <returns>Whether validation was succesful and reason of fail.</returns>
        /// <exception cref="ArgumentNullException">Thrown when first name is null.</exception>
        public static Tuple<bool, string> FirstNameDefaultValidation(string firstName)
        {
            if (firstName is null)
            {
                throw new ArgumentNullException(nameof(firstName), "First name must be not null.");
            }

            if (firstName.Length < 2 || firstName.Length > 60)
            {
                return new Tuple<bool, string>(false, "First name length must be from 2 to 60 chars");
            }

            if (firstName.Trim().Length == 0)
            {
                return new Tuple<bool, string>(false, "First name can not contain only whitespaces");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        /// <summary>
        /// Validates first name.
        /// </summary>
        /// <param name="firstName">First name to validate.</param>
        /// <returns>Whether validation was succesful and reason of fail.</returns>
        /// <exception cref="ArgumentNullException">Thrown when first name is null.</exception>
        public static Tuple<bool, string> FirstNameCustomValidation(string firstName)
        {
            if (firstName is null)
            {
                throw new ArgumentNullException(nameof(firstName), "First name must be not null.");
            }

            if (firstName.Length < 1 || firstName.Length > 40)
            {
                return new Tuple<bool, string>(false, "First name length must be from 1 to 40 chars");
            }

            if (firstName.Trim().Length == 0)
            {
                return new Tuple<bool, string>(false, "First name can not contain only whitespaces");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        /// <summary>
        /// Validates last name.
        /// </summary>
        /// <param name="lastName">Last name to validate.</param>
        /// <returns>Whether validation was succesful and reason of fail.</returns>
        /// <exception cref="ArgumentNullException">Thrown when last name is null.</exception>
        public static Tuple<bool, string> LastNameDefaultValidation(string lastName)
        {
            if (lastName is null)
            {
                throw new ArgumentNullException(nameof(lastName), "Last name must be not null.");
            }

            if (lastName.Length < 2 || lastName.Length > 60)
            {
                return new Tuple<bool, string>(false, "Last name length must be from 2 to 60 chars");
            }

            if (lastName.Trim().Length == 0)
            {
                return new Tuple<bool, string>(false, "Last name can not contain only whitespaces");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        /// <summary>
        /// Validates last name.
        /// </summary>
        /// <param name="lastName">Last name to validate.</param>
        /// <returns>Whether validation was succesful and reason of fail.</returns>
        /// <exception cref="ArgumentNullException">Thrown when last name is null.</exception>
        public static Tuple<bool, string> LastNameCustomValidation(string lastName)
        {
            if (lastName is null)
            {
                throw new ArgumentNullException(nameof(lastName), "Last name must be not null.");
            }

            if (lastName.Length < 1 || lastName.Length > 40)
            {
                return new Tuple<bool, string>(false, "Last name length must be from 1 to 40 chars");
            }

            if (lastName.Trim().Length == 0)
            {
                return new Tuple<bool, string>(false, "Last name can not contain only whitespaces");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        /// <summary>
        /// Validates date of birth.
        /// </summary>
        /// <param name="dateOfBirth">Date of birth to validate.</param>
        /// <returns>Whether validation was succesful and reason of fail.</returns>
        public static Tuple<bool, string> DateOfBirthDefaultValidation(DateTime dateOfBirth)
        {
            if (dateOfBirth < new DateTime(1950, 1, 1) || dateOfBirth > DateTime.Today)
            {
                return new Tuple<bool, string>(false, "Date of birth must be from 01-Jan-1950 to today");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        /// <summary>
        /// Validates date of birth.
        /// </summary>
        /// <param name="dateOfBirth">Date of birth to validate.</param>
        /// <returns>Whether validation was succesful and reason of fail.</returns>
        public static Tuple<bool, string> DateOfBirthCustomValidation(DateTime dateOfBirth)
        {
            if (dateOfBirth < new DateTime(1900, 1, 1) || dateOfBirth > DateTime.Today)
            {
                return new Tuple<bool, string>(false, "Date of birth must be from 01-Jan-1900 to today");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        /// <summary>
        /// Validates height.
        /// </summary>
        /// <param name="height">Height to validate.</param>
        /// <returns>Whether validation was succesful and reason of fail.</returns>
        public static Tuple<bool, string> HeightDefaultValidation(short height)
        {
            if (height <= 0 || height > 300)
            {
                return new Tuple<bool, string>(false, "Height must be from 1 to 300 cm");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        /// <summary>
        /// Validates height.
        /// </summary>
        /// <param name="height">Height to validate.</param>
        /// <returns>Whether validation was succesful and reason of fail.</returns>
        public static Tuple<bool, string> HeightCustomValidation(short height)
        {
            if (height < 10 || height > 280)
            {
                return new Tuple<bool, string>(false, "Height must be from 10 to 280 cm");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        /// <summary>
        /// Validates income.
        /// </summary>
        /// <param name="income">Income to validate.</param>
        /// <returns>Whether validation was succesful and reason of fail.</returns>
        public static Tuple<bool, string> IncomeDefaultValidation(decimal income)
        {
            if (income < 0)
            {
                return new Tuple<bool, string>(false, "Income must be not negative number");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        /// <summary>
        /// Validates income.
        /// </summary>
        /// <param name="income">Income to validate.</param>
        /// <returns>Whether validation was succesful and reason of fail.</returns>
        public static Tuple<bool, string> IncomeCustomValidation(decimal income)
        {
            if (income < 0 || income > 999999999)
            {
                return new Tuple<bool, string>(false, "Income must be not negative number less than 999999999");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        /// <summary>
        /// Validates patronymic letter.
        /// </summary>
        /// <param name="patronymicLetter">Patronymic letter to validate.</param>
        /// <returns>Whether validation was succesful and reason of fail.</returns>
        public static Tuple<bool, string> PatronymicLetterDefaultValidation(char patronymicLetter)
        {
            if (patronymicLetter < 'A' || patronymicLetter > 'Z')
            {
                return new Tuple<bool, string>(false, "Patronymic letter must be a latin letter in uppercase");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        /// <summary>
        /// Validates patronymic letter.
        /// </summary>
        /// <param name="patronymicLetter">Patronymic letter to validate.</param>
        /// <returns>Whether validation was succesful and reason of fail.</returns>
        public static Tuple<bool, string> PatronymicLetterCustomValidation(char patronymicLetter)
        {
            if (patronymicLetter < 'A' || patronymicLetter > 'Z')
            {
                return new Tuple<bool, string>(false, "Patronymic letter must be a latin letter in uppercase");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }
    }
}
