using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides type to describe a record.
    /// </summary>
    public class FileCabinetRecord
    {
        /// <summary>
        /// Gets or sets unique ID of a record.
        /// </summary>
        /// <value>Value of ID of a record.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets first name of record subject.
        /// </summary>
        /// <value>Value of first name of a record.</value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets last name of record subject.
        /// </summary>
        /// <value>Value of last name of a record.</value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets date of birth of record subject.
        /// </summary>
        /// <value>Value of date of birth of a record.</value>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets height of record subject.
        /// </summary>
        /// <value>Value of height of a record.</value>
        public short Height { get; set; }

        /// <summary>
        /// Gets or sets income of record subject.
        /// </summary>
        /// <value>Value of income of a record.</value>
        public decimal Income { get; set; }

        /// <summary>
        /// Gets or sets patronymic letter of record subject.
        /// </summary>
        /// <value>Value of patronymic letter of a record.</value>
        public char PatronymicLetter { get; set; }
    }
}
