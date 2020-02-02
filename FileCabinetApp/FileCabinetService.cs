using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short height, decimal income, char patronymicLetter)
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
            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                this.firstNameDictionary[firstName].Add(record);
            }
            else
            {
                this.firstNameDictionary.Add(firstName, new List<FileCabinetRecord>());
                this.firstNameDictionary[firstName].Add(record);
            }

            return record.Id;
        }

        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, short height, decimal income, char patronymicLetter)
        {
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
                    this.list[i] = editedRecord;
                    break;
                }

                if (i == this.list.Count - 1)
                {
                    throw new ArgumentException($"#{id} record is not found.", nameof(id));
                }
            }

            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                for (int j = 0; j < this.firstNameDictionary[firstName].Count; j++)
                {
                    if (this.firstNameDictionary[firstName][j].Id == id)
                    {
                        this.firstNameDictionary[firstName][j] = editedRecord;
                        break;
                    }
                }
            }
            else
            {
                this.firstNameDictionary.Add(firstName, new List<FileCabinetRecord>());
                this.firstNameDictionary[firstName].Add(editedRecord);
            }

            Console.WriteLine($"Record #{id} is updated.");
        }

        public FileCabinetRecord[] GetRecords()
        {
            FileCabinetRecord[] fileCabinetRecords = this.list.ToArray();
            return fileCabinetRecords;
        }

        public int GetStat()
        {
            return this.list.Count;
        }

        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            List<FileCabinetRecord> records;
            if (!this.firstNameDictionary.TryGetValue(firstName, out records))
            {
                return Array.Empty<FileCabinetRecord>();
            }

            return records.ToArray();
        }

        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();
            foreach (FileCabinetRecord record in this.list)
            {
                if (record.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase))
                {
                    records.Add(record);
                }
            }

            return records.ToArray();
        }

        public FileCabinetRecord[] FindByDateOfbirth(string dateOfBirth)
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();
            foreach (FileCabinetRecord record in this.list)
            {
                if (record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture).Equals(dateOfBirth, StringComparison.OrdinalIgnoreCase))
                {
                    records.Add(record);
                }
            }

            return records.ToArray();
        }
    }
}
