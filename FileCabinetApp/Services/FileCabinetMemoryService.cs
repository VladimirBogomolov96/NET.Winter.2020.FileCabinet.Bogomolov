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
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();
        private IRecordValidator recordValidator;
        private List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="recordValidator">Validator for parameters.</param>
        public FileCabinetMemoryService(IRecordValidator recordValidator)
        {
            this.recordValidator = recordValidator;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        public FileCabinetMemoryService()
        {
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
                Id = this.list[this.list.Count - 1].Id + 1,
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
                    this.RemoveFromDictionaries(this.list[i]);
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

        /// <summary>
        /// Removes record by given id.
        /// </summary>
        /// <param name="id">ID of record to remove.</param>
        /// <returns>Whether record existed or not.</returns>
        public bool Remove(int id)
        {
            foreach (FileCabinetRecord record in this.list)
            {
                if (record.Id == id)
                {
                    this.list.Remove(record);
                    this.RemoveFromDictionaries(record);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Restores statement from snapshot.
        /// </summary>
        /// <param name="snapshot">Snapshot that represent statement to restore.</param>
        /// <returns>Amount of new records added.</returns>
        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot is null)
            {
                throw new ArgumentNullException(nameof(snapshot), "Snapshot must be not null.");
            }

            List<FileCabinetRecord> resultRecords = new List<FileCabinetRecord>();
            ReadOnlyCollection<FileCabinetRecord> importData = snapshot.GetRecords;
            int sourceIndex = 0;
            int importIndex = 0;

            for (; sourceIndex < this.list.Count && importIndex < importData.Count;)
            {
                if (this.list[sourceIndex].Id < importData[importIndex].Id)
                {
                    resultRecords.Add(this.list[sourceIndex]);
                    sourceIndex++;
                }
                else if (this.list[sourceIndex].Id == importData[importIndex].Id)
                {
                    try
                    {
                        RecordParametersTransfer transfer = new RecordParametersTransfer(importData[importIndex].FirstName, importData[importIndex].LastName, importData[importIndex].DateOfBirth, importData[importIndex].Height, importData[importIndex].Income, importData[importIndex].PatronymicLetter);
                        this.recordValidator.ValidateParameters(transfer);
                        this.RemoveFromDictionaries(this.list[sourceIndex]);
                        this.FillDictionaries(transfer, importData[importIndex]);
                        resultRecords.Add(importData[importIndex]);
                        importIndex++;
                        sourceIndex++;
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Wrong data in record #{0} : {1}", importData[importIndex].Id, ex.Message));
                        importIndex++;
                        sourceIndex++;
                        continue;
                    }
                }
                else
                {
                    try
                    {
                        RecordParametersTransfer transfer = new RecordParametersTransfer(importData[importIndex].FirstName, importData[importIndex].LastName, importData[importIndex].DateOfBirth, importData[importIndex].Height, importData[importIndex].Income, importData[importIndex].PatronymicLetter);
                        this.recordValidator.ValidateParameters(transfer);
                        resultRecords.Add(importData[importIndex]);
                        this.FillDictionaries(transfer, importData[importIndex]);
                        importIndex++;
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Wrong data in record #{0} : {1}", importData[importIndex].Id, ex.Message));
                        importIndex++;
                        continue;
                    }
                }
            }

            for (; importIndex < importData.Count; importIndex++)
            {
                try
                {
                    RecordParametersTransfer transfer = new RecordParametersTransfer(importData[importIndex].FirstName, importData[importIndex].LastName, importData[importIndex].DateOfBirth, importData[importIndex].Height, importData[importIndex].Income, importData[importIndex].PatronymicLetter);
                    this.recordValidator.ValidateParameters(transfer);
                    resultRecords.Add(importData[importIndex]);
                    this.FillDictionaries(transfer, importData[importIndex]);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Wrong data in record #{0} : {1}", importData[importIndex].Id, ex.Message));
                    continue;
                }
            }

            for (; sourceIndex < this.list.Count; sourceIndex++)
            {
                resultRecords.Add(this.list[sourceIndex]);
            }

            this.list = resultRecords;

            return importIndex;
        }

        /// <summary>
        /// Sets record validator.
        /// </summary>
        /// <param name="recordValidator">Rules of validation.</param>
        public void SetRecordValidator(IRecordValidator recordValidator)
        {
            this.recordValidator = recordValidator;
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

        private void RemoveFromDictionaries(FileCabinetRecord record)
        {
            this.firstNameDictionary[record.FirstName].Remove(record);
            if (this.firstNameDictionary[record.FirstName].Count is 0)
            {
                this.firstNameDictionary.Remove(record.FirstName);
            }

            this.lastNameDictionary[record.LastName].Remove(record);
            if (this.lastNameDictionary[record.LastName].Count is 0)
            {
                this.lastNameDictionary.Remove(record.LastName);
            }

            this.dateOfBirthDictionary[record.DateOfBirth].Remove(record);
            if (this.dateOfBirthDictionary[record.DateOfBirth].Count is 0)
            {
                this.dateOfBirthDictionary.Remove(record.DateOfBirth);
            }
        }
    }
}
