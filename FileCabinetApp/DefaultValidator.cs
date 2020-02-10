using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides method for parameters validation.
    /// </summary>
    public class DefaultValidator : IRecordValidator
    {
        /// <summary>
        /// Validate transfer parameters to use in methods.
        /// </summary>
        /// <param name="transfer">Transfer parameters to validate.</param>
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

            if (transfer.FirstName.Length < 2 || transfer.FirstName.Length > 60)
            {
                throw new ArgumentException("First name length must be from 2 to 60 chars.", nameof(transfer));
            }

            if (transfer.FirstName.Trim().Length == 0)
            {
                throw new ArgumentException("First name can not contain only whitespaces.", nameof(transfer));
            }

            if (transfer.LastName is null)
            {
                throw new ArgumentNullException(nameof(transfer), "Last name can not be null.");
            }

            if (transfer.LastName.Length < 2 || transfer.LastName.Length > 60)
            {
                throw new ArgumentException("Last name length must be from 2 to 60 chars.", nameof(transfer));
            }

            if (transfer.LastName.Trim().Length == 0)
            {
                throw new ArgumentException("Last name can not contain only whitespaces.", nameof(transfer));
            }

            if (transfer.DateOfBirth < new DateTime(1950, 1, 1) || transfer.DateOfBirth > DateTime.Today)
            {
                throw new ArgumentException("Date of birth must be from 01-Jan-1950 to today.", nameof(transfer));
            }

            if (transfer.Height <= 0 || transfer.Height > 300)
            {
                throw new ArgumentException("Height must be from 1 to 300 cm.", nameof(transfer));
            }

            if (transfer.Income < 0)
            {
                throw new ArgumentException("Income must be not negative number.", nameof(transfer));
            }

            if (transfer.PatronymicLetter < 'A' || transfer.PatronymicLetter > 'Z')
            {
                throw new ArgumentException("Patronymic letter must be a latin letter in uppercase.", nameof(transfer));
            }
        }
    }
}
