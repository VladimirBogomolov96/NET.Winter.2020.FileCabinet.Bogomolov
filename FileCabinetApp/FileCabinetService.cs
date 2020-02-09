using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides methods to interact with records.
    /// </summary>
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();

        /// <summary>
        /// Creates new records with given parameters.
        /// </summary>
        /// <param name="firstName">First name to set to new record.</param>
        /// <param name="lastName">Last name to set to new record.</param>
        /// <param name="dateOfBirth">Date of birth to set to new record.</param>
        /// <param name="height">Height to set to new record.</param>
        /// <param name="income">Income to set to new record.</param>
        /// <param name="patronymicLetter">Patronymic letter to set to new record.</param>
        /// <returns>ID of created record.</returns>
        /// <exception cref="ArgumentNullException">Throw when first name or last name is null.</exception>
        /// <exception cref="ArgumentException">Thrown when firs name or last name length is out of 2 and 60 chars or contains only whitespaces, when date of birth out of 01-Jan-1950 and current date, when height is out of 1 and 300 cm, when income is negative, when patronymic letter is not a latin uppercase letter.</exception>
        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short height, decimal income, char patronymicLetter)
        {
            ValidateParameters(firstName, lastName, dateOfBirth, height, income, patronymicLetter);

            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Height = height,
                Income = income,
                PatronymicLetter = patronymicLetter,
            };

            this.list.Add(record);
            this.FillDictionaries(firstName, lastName, dateOfBirth, record);

            return record.Id;
        }

        /// <summary>
        /// Edits existing record.
        /// </summary>
        /// <param name="id">ID of a record to edit.</param>
        /// <param name="firstName">New first name to set to editing record.</param>
        /// <param name="lastName">New last name to set to editing record.</param>
        /// <param name="dateOfBirth">New date of birth to set to editing record.</param>
        /// <param name="height">New height to set to editing record.</param>
        /// <param name="income">New income to set to editing record.</param>
        /// <param name="patronymicLetter">New patronymic letter to set to editing record.</param>
        /// <exception cref="ArgumentNullException">Throw when first name or last name is null.</exception>
        /// <exception cref="ArgumentException">Thrown when firs name or last name length is out of 2 and 60 chars or contains only whitespaces, when date of birth out of 01-Jan-1950 and current date, when height is out of 1 and 300 cm, when income is negative, when patronymic letter is not a latin uppercase letter.</exception>
        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, short height, decimal income, char patronymicLetter)
        {
            ValidateParameters(firstName, lastName, dateOfBirth, height, income, patronymicLetter);
            FileCabinetRecord editedRecord = new FileCabinetRecord()
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Height = height,
                Income = income,
                PatronymicLetter = patronymicLetter,
            };
            for (int i = 0; i < this.list.Count; i++)
            {
                if (this.list[i].Id == id)
                {
                    this.firstNameDictionary[this.list[i].FirstName].Remove(this.list[i]);
                    this.lastNameDictionary[this.list[i].LastName].Remove(this.list[i]);
                    this.dateOfBirthDictionary[this.list[i].DateOfBirth].Remove(this.list[i]);
                    this.list[i] = editedRecord;
                    break;
                }

                if (i == this.list.Count - 1)
                {
                    throw new ArgumentException($"#{id} record is not found.", nameof(id));
                }
            }

            this.FillDictionaries(firstName, lastName, dateOfBirth, editedRecord);
            Console.WriteLine($"Record #{id} is updated.");
        }

        /// <summary>
        /// Gets all existing records.
        /// </summary>
        /// <returns>Array of all existing records.</returns>
        public FileCabinetRecord[] GetRecords()
        {
            FileCabinetRecord[] fileCabinetRecords = this.list.ToArray();
            return fileCabinetRecords;
        }

        /// <summary>
        /// Counts amount of existing records.
        /// </summary>
        /// <returns>Amount of existing records.</returns>
        public int GetStat()
        {
            return this.list.Count;
        }

        /// <summary>
        /// Finds all records with given first name.
        /// </summary>
        /// <param name="firstName">First name to match with.</param>
        /// <returns>Array of matching records.</returns>
        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            if (!this.firstNameDictionary.TryGetValue(firstName, out List<FileCabinetRecord> records))
            {
                return Array.Empty<FileCabinetRecord>();
            }

            return records.ToArray();
        }

        /// <summary>
        /// Finds all records with given last name.
        /// </summary>
        /// <param name="lastName">Last name to match with.</param>
        /// <returns>Array of matching records.</returns>
        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            if (!this.lastNameDictionary.TryGetValue(lastName, out List<FileCabinetRecord> records))
            {
                return Array.Empty<FileCabinetRecord>();
            }

            return records.ToArray();
        }

        /// <summary>
        /// Finds all records with given date of birth.
        /// </summary>
        /// <param name="dateOfBirth">Date of birth to match with.</param>
        /// <returns>Array of matching records.</returns>
        public FileCabinetRecord[] FindByDateOfbirth(DateTime dateOfBirth)
        {
            if (!this.dateOfBirthDictionary.TryGetValue(dateOfBirth, out List<FileCabinetRecord> records))
            {
                return Array.Empty<FileCabinetRecord>();
            }

            return records.ToArray();
        }

        private static void ValidateParameters(string firstName, string lastName, DateTime dateOfBirth, short height, decimal income, char patronymicLetter)
        {
            if (firstName is null)
            {
                throw new ArgumentNullException(nameof(firstName), "First name can not be null.");
            }

            if (firstName.Length < 2 || firstName.Length > 60)
            {
                throw new ArgumentException("First name length must be from 2 to 60 chars.", nameof(firstName));
            }

            if (firstName.Trim().Length == 0)
            {
                throw new ArgumentException("First name can not contain only whitespaces.", nameof(firstName));
            }

            if (lastName is null)
            {
                throw new ArgumentNullException(nameof(lastName), "Last name can not be null.");
            }

            if (lastName.Length < 2 || lastName.Length > 60)
            {
                throw new ArgumentException("Last name length must be from 2 to 60 chars.", nameof(lastName));
            }

            if (lastName.Trim().Length == 0)
            {
                throw new ArgumentException("Last name can not contain only whitespaces.", nameof(lastName));
            }

            if (dateOfBirth < new DateTime(1950, 1, 1) || dateOfBirth > DateTime.Today)
            {
                throw new ArgumentException("Date of birth must be from 01-Jan-1950 to today.", nameof(dateOfBirth));
            }

            if (height <= 0 || height > 300)
            {
                throw new ArgumentException("Height must be from 1 to 300 cm.", nameof(height));
            }

            if (income < 0)
            {
                throw new ArgumentException("Income must be not negative number.", nameof(income));
            }

            if (patronymicLetter < 'A' || patronymicLetter > 'Z')
            {
                throw new ArgumentException("Patronymic letter must be a latin letter in uppercase.");
            }
        }

        private void FillDictionaries(string firstName, string lastName, DateTime dateOfBirth, FileCabinetRecord record)
        {
            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                this.firstNameDictionary[firstName].Add(record);
            }
            else
            {
                this.firstNameDictionary.Add(firstName, new List<FileCabinetRecord>());
                this.firstNameDictionary[firstName].Add(record);
            }

            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                this.lastNameDictionary[lastName].Add(record);
            }
            else
            {
                this.lastNameDictionary.Add(lastName, new List<FileCabinetRecord>());
                this.lastNameDictionary[lastName].Add(record);
            }

            if (this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
            {
                this.dateOfBirthDictionary[dateOfBirth].Add(record);
            }
            else
            {
                this.dateOfBirthDictionary.Add(dateOfBirth, new List<FileCabinetRecord>());
                this.dateOfBirthDictionary[dateOfBirth].Add(record);
            }
        }
    }
}
