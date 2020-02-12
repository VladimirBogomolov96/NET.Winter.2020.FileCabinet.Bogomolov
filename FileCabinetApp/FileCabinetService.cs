using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides methods to interact with records.
    /// </summary>
    public class FileCabinetService : IFileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();
        private IRecordValidator recordValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetService"/> class.
        /// </summary>
        /// <param name="recordValidator">Validator for parameters.</param>
        public FileCabinetService(IRecordValidator recordValidator)
        {
            this.recordValidator = recordValidator;
        }

        /// <summary>
        /// Creates new records with given parameters.
        /// </summary>
        /// <param name="transfer">Object to transfer parameters of new record.</param>
        /// <returns>ID of created record.</returns>
        /// <exception cref="ArgumentNullException">Throw when first name or last name is null, when transfer object is null.</exception>
        /// <exception cref="ArgumentException">Thrown when firs name or last name length is out of 2 and 60 chars or contains only whitespaces, when date of birth out of 01-Jan-1950 and current date, when height is out of 1 and 300 cm, when income is negative, when patronymic letter is not a latin uppercase letter.</exception>
        public int CreateRecord(RecordParametersTransfer transfer)
        {
            this.recordValidator.ValidateParameters(transfer);
            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = transfer.FirstName,
                LastName = transfer.LastName,
                DateOfBirth = transfer.DateOfBirth,
                Height = transfer.Height,
                Income = transfer.Income,
                PatronymicLetter = transfer.PatronymicLetter,
            };

            this.list.Add(record);
            this.FillDictionaries(transfer, record);
            return record.Id;
        }

        /// <summary>
        /// Edits existing record.
        /// </summary>
        /// <param name="id">ID of a record to edit.</param>
        /// <param name="transfer">Object to transfer new parameters to existing record.</param>
        /// <exception cref="ArgumentNullException">Throw when first name or last name is null, when transfer object is null.</exception>
        /// <exception cref="ArgumentException">Thrown when firs name or last name length is out of 2 and 60 chars or contains only whitespaces, when date of birth out of 01-Jan-1950 and current date, when height is out of 1 and 300 cm, when income is negative, when patronymic letter is not a latin uppercase letter.</exception>
        public void EditRecord(int id, RecordParametersTransfer transfer)
        {
            this.recordValidator.ValidateParameters(transfer);
            FileCabinetRecord editedRecord = new FileCabinetRecord()
            {
                Id = id,
                FirstName = transfer.FirstName,
                LastName = transfer.LastName,
                DateOfBirth = transfer.DateOfBirth,
                Height = transfer.Height,
                Income = transfer.Income,
                PatronymicLetter = transfer.PatronymicLetter,
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

            this.FillDictionaries(transfer, editedRecord);
            Console.WriteLine($"Record #{id} is updated.");
        }

        /// <summary>
        /// Gets all existing records.
        /// </summary>
        /// <returns>Array of all existing records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return new ReadOnlyCollection<FileCabinetRecord>(this.list);
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
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (!this.firstNameDictionary.TryGetValue(firstName, out List<FileCabinetRecord> records))
            {
                return new ReadOnlyCollection<FileCabinetRecord>(Array.Empty<FileCabinetRecord>());
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <summary>
        /// Finds all records with given last name.
        /// </summary>
        /// <param name="lastName">Last name to match with.</param>
        /// <returns>Array of matching records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (!this.lastNameDictionary.TryGetValue(lastName, out List<FileCabinetRecord> records))
            {
                return new ReadOnlyCollection<FileCabinetRecord>(Array.Empty<FileCabinetRecord>());
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <summary>
        /// Finds all records with given date of birth.
        /// </summary>
        /// <param name="dateOfBirth">Date of birth to match with.</param>
        /// <returns>Array of matching records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfbirth(DateTime dateOfBirth)
        {
            if (!this.dateOfBirthDictionary.TryGetValue(dateOfBirth, out List<FileCabinetRecord> records))
            {
                return new ReadOnlyCollection<FileCabinetRecord>(Array.Empty<FileCabinetRecord>());
            }

            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <summary>
        /// Creates a snapshot of all records in current moment.
        /// </summary>
        /// <returns>Snapshot of records.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.list.ToArray());
        }

        private void FillDictionaries(RecordParametersTransfer transfer, FileCabinetRecord record)
        {
            if (this.firstNameDictionary.ContainsKey(transfer.FirstName))
            {
                this.firstNameDictionary[transfer.FirstName].Add(record);
            }
            else
            {
                this.firstNameDictionary.Add(transfer.FirstName, new List<FileCabinetRecord>());
                this.firstNameDictionary[transfer.FirstName].Add(record);
            }

            if (this.lastNameDictionary.ContainsKey(transfer.LastName))
            {
                this.lastNameDictionary[transfer.LastName].Add(record);
            }
            else
            {
                this.lastNameDictionary.Add(transfer.LastName, new List<FileCabinetRecord>());
                this.lastNameDictionary[transfer.LastName].Add(record);
            }

            if (this.dateOfBirthDictionary.ContainsKey(transfer.DateOfBirth))
            {
                this.dateOfBirthDictionary[transfer.DateOfBirth].Add(record);
            }
            else
            {
                this.dateOfBirthDictionary.Add(transfer.DateOfBirth, new List<FileCabinetRecord>());
                this.dateOfBirthDictionary[transfer.DateOfBirth].Add(record);
            }
        }
    }
}
