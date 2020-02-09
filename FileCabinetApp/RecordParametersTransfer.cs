using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Parameters transition class.
    /// </summary>
    public class RecordParametersTransfer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecordParametersTransfer"/> class.
        /// </summary>
        /// <param name="firstName">First name parameter to transfer.</param>
        /// <param name="lastName">Last name parameter to transfer.</param>
        /// <param name="dateOfBirth">Date of birth parameter to transfer.</param>
        /// <param name="height">Height parameter to transfer.</param>
        /// <param name="income">Income parameter to transfer.</param>
        /// <param name="patronymicLetter">Patronymic letter parameter to transfer.</param>
        public RecordParametersTransfer(string firstName, string lastName, DateTime dateOfBirth, short height, decimal income, char patronymicLetter)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.DateOfBirth = dateOfBirth;
            this.Height = height;
            this.Income = income;
            this.PatronymicLetter = patronymicLetter;
        }

        /// <summary>
        /// Gets first name to transfer.
        /// </summary>
        /// <value>Value of first name to transfer.</value>
        public string FirstName { get; }

        /// <summary>
        /// Gets last name to transfer.
        /// </summary>
        /// <value>Value of last name to transfer.</value>
        public string LastName { get; }

        /// <summary>
        /// Gets date of birth to transfer.
        /// </summary>
        /// <value>Value of date of birth to transfer.</value>
        public DateTime DateOfBirth { get; }

        /// <summary>
        /// Gets height to transfer.
        /// </summary>
        /// <value>Value of height to transfer.</value>
        public short Height { get; }

        /// <summary>
        /// Gets income to transfer.
        /// </summary>
        /// <value>Value of income to transfer.</value>
        public decimal Income { get; }

        /// <summary>
        /// Gets patronymic letter to transfer.
        /// </summary>
        /// <value>Value of patronymic letter to transfer.</value>
        public char PatronymicLetter { get; }
    }
}
