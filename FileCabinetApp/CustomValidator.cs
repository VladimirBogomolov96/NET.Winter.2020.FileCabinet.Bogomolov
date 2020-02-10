using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides method for parameters validation.
    /// </summary>
    public class CustomValidator : IRecordValidator
    {
        /// <summary>
        /// Validate given parameters.
        /// </summary>
        /// <param name="transfer">Transfer of parameters to validate.</param>
        public void ValidateParameters(RecordParametersTransfer transfer)
        {
            if (transfer is null)
            {
                throw new ArgumentNullException(nameof(transfer), "Transfer object must be not null.");
            }

            if (transfer.FirstName is null)
            {
                throw new ArgumentNullException(nameof(transfer), "First name can not be null.");
            }

            if (transfer.FirstName.Length < 1 || transfer.FirstName.Length > 40)
            {
                throw new ArgumentException("First name length must be from 1 to 40 chars.", nameof(transfer));
            }

            if (transfer.FirstName.Trim().Length == 0)
            {
                throw new ArgumentException("First name can not contain only whitespaces.", nameof(transfer));
            }

            if (transfer.LastName is null)
            {
                throw new ArgumentNullException(nameof(transfer), "Last name can not be null.");
            }

            if (transfer.LastName.Length < 1 || transfer.LastName.Length > 40)
            {
                throw new ArgumentException("Last name length must be from 1 to 40 chars.", nameof(transfer));
            }

            if (transfer.LastName.Trim().Length == 0)
            {
                throw new ArgumentException("Last name can not contain only whitespaces.", nameof(transfer));
            }

            if (transfer.DateOfBirth < new DateTime(1900, 1, 1) || transfer.DateOfBirth > DateTime.Today)
            {
                throw new ArgumentException("Date of birth must be from 01-Jan-1900 to today.", nameof(transfer));
            }

            if (transfer.Height < 10 || transfer.Height > 280)
            {
                throw new ArgumentException("Height must be from 10 to 280 cm.", nameof(transfer));
            }

            if (transfer.Income < 0 || transfer.Income > 999999999)
            {
                throw new ArgumentException("Income must be not negative number less than 999999999.", nameof(transfer));
            }

            if (transfer.PatronymicLetter < 'A' || transfer.PatronymicLetter > 'Z')
            {
                throw new ArgumentException("Patronymic letter must be a latin letter in uppercase.", nameof(transfer));
            }
        }
    }
}
