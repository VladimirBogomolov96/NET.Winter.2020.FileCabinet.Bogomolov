﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// First name validator.
    /// </summary>
    public class FirstNameValidator : IRecordValidator
    {
        private int minLength;
        private int maxLength;

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
                throw new ArgumentNullException(nameof(record), "Record must be not null.");
            }

            if (record.FirstName is null || record.FirstName.Length < this.minLength || record.FirstName.Length > this.maxLength || record.FirstName.Trim().Length is 0)
            {
                return new Tuple<bool, string>(false, "Wrong first name.");
            }

            return new Tuple<bool, string>(true, null);
        }
    }
}