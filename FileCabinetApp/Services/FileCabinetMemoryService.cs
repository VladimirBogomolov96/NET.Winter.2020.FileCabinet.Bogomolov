using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
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
        private readonly List<int> ids = new List<int>();
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
            if (transfer is null)
            {
                throw new ArgumentNullException(nameof(transfer), "Transfer must be not null.");
            }

            if (!this.recordValidator.ValidateParameters(transfer.RecordSimulation()).Item1)
            {
                throw new ArgumentException(this.recordValidator.ValidateParameters(transfer.RecordSimulation()).Item2);
            }

            var record = new FileCabinetRecord
            {
                FirstName = transfer.FirstName,
                LastName = transfer.LastName,
                DateOfBirth = transfer.DateOfBirth,
                Height = transfer.Height,
                Income = transfer.Income,
                PatronymicLetter = transfer.PatronymicLetter,
            };

            if (this.list.Count is 0)
            {
                record.Id = 1;
            }
            else
            {
                record.Id = this.ids.Max() + 1;
            }

            this.list.Add(record);
            this.ids.Add(record.Id);
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
            if (transfer is null)
            {
                throw new ArgumentNullException(nameof(transfer), "Transfer must be not null.");
            }

            if (!this.recordValidator.ValidateParameters(transfer.RecordSimulation()).Item1)
            {
                throw new ArgumentException(this.recordValidator.ValidateParameters(transfer.RecordSimulation()).Item2);
            }

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
        public (int, int) GetStat()
        {
            return (this.list.Count, 0);
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
        /// Deletes records.
        /// </summary>
        /// <param name="records">Records to delete.</param>
        /// <returns>IDs of deleted records.</returns>
        public IEnumerable<int> Delete(IEnumerable<FileCabinetRecord> records)
        {
            if (records is null)
            {
                throw new ArgumentNullException(nameof(records), "Records must be not null.");
            }

            foreach (FileCabinetRecord record in records)
            {
                this.Remove(record.Id);
                yield return record.Id;
            }
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
                    this.ids.Remove(id);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Inserts new record.
        /// </summary>
        /// <param name="record">Record to insert.</param>
        /// <returns>Id of inserted record.</returns>
        public int Insert(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), "Record must be not null.");
            }

            if (!this.recordValidator.ValidateParameters(record).Item1)
            {
                throw new ArgumentException(this.recordValidator.ValidateParameters(record).Item2, nameof(record));
            }

            if (this.ids.Contains(record.Id))
            {
                throw new ArgumentException("Record with given id already exists.", nameof(record));
            }

            this.list.Add(record);
            this.ids.Add(record.Id);
            this.FillDictionaries(record);
            return record.Id;
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
                        RecordParametersTransfer transfer = new RecordParametersTransfer(
                            importData[importIndex].FirstName,
                            importData[importIndex].LastName,
                            importData[importIndex].DateOfBirth,
                            importData[importIndex].Height,
                            importData[importIndex].Income,
                            importData[importIndex].PatronymicLetter);
                        if (!this.recordValidator.ValidateParameters(transfer.RecordSimulation()).Item1)
                        {
                            throw new ArgumentException(this.recordValidator.ValidateParameters(transfer.RecordSimulation()).Item2);
                        }

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
                        RecordParametersTransfer transfer = new RecordParametersTransfer(
                            importData[importIndex].FirstName,
                            importData[importIndex].LastName,
                            importData[importIndex].DateOfBirth,
                            importData[importIndex].Height,
                            importData[importIndex].Income,
                            importData[importIndex].PatronymicLetter);
                        if (!this.recordValidator.ValidateParameters(transfer.RecordSimulation()).Item1)
                        {
                            throw new ArgumentException(this.recordValidator.ValidateParameters(transfer.RecordSimulation()).Item2);
                        }

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
                    RecordParametersTransfer transfer = new RecordParametersTransfer(
                        importData[importIndex].FirstName,
                        importData[importIndex].LastName,
                        importData[importIndex].DateOfBirth,
                        importData[importIndex].Height,
                        importData[importIndex].Income,
                        importData[importIndex].PatronymicLetter);
                    if (!this.recordValidator.ValidateParameters(transfer.RecordSimulation()).Item1)
                    {
                        throw new ArgumentException(this.recordValidator.ValidateParameters(transfer.RecordSimulation()).Item2);
                    }

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

        /// <summary>
        /// Defragments file.
        /// </summary>
        /// <returns>Amount of purged records.</returns>
        public int Purge()
        {
            throw new InvalidOperationException("Purge command can't be used in memory sevice.");
        }

        /// <summary>
        /// Updates records.
        /// </summary>
        /// <param name="records">Records to update.</param>
        /// <param name="fieldsAndValuesToSet">Fields and values to set.</param>
        /// <returns>Amount of updated records.</returns>
        public int Update(IEnumerable<FileCabinetRecord> records, IEnumerable<IEnumerable<string>> fieldsAndValuesToSet)
        {
            if (records is null)
            {
                throw new ArgumentNullException(nameof(records), "Records must be not null.");
            }

            if (fieldsAndValuesToSet is null)
            {
                throw new ArgumentNullException(nameof(fieldsAndValuesToSet), "Fields and values to set must be not null.");
            }

            int result = 0;
            foreach (var record in records)
            {
                for (int i = 0; i < this.list.Count; i++)
                {
                    if (record.Id == this.list[i].Id)
                    {
                        FileCabinetRecord temp = new FileCabinetRecord()
                        {
                            Id = record.Id,
                            FirstName = record.FirstName,
                            LastName = record.LastName,
                            DateOfBirth = record.DateOfBirth,
                            Height = record.Height,
                            Income = record.Income,
                            PatronymicLetter = record.PatronymicLetter,
                        };
                        this.RemoveFromDictionaries(record);

                        try
                        {
                            this.UpdateRecord(record, fieldsAndValuesToSet);
                            this.FillDictionaries(record);
                            result++;
                        }
                        catch (ArgumentException ex)
                        {
                            record.Id = temp.Id;
                            record.FirstName = temp.FirstName;
                            record.LastName = temp.LastName;
                            record.DateOfBirth = temp.DateOfBirth;
                            record.Income = temp.Income;
                            record.Height = temp.Height;
                            record.PatronymicLetter = temp.PatronymicLetter;
                            this.FillDictionaries(record);
                            throw new ArgumentException(ex.Message);
                        }
                    }
                }
            }

            return result;
        }

        private void UpdateRecord(FileCabinetRecord record, IEnumerable<IEnumerable<string>> fieldsAndValuesToSet)
        {
            foreach (var keyValuePair in fieldsAndValuesToSet)
            {
                var key = keyValuePair.First();
                var value = keyValuePair.Last();
                if (key.Equals("id", StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new ArgumentException("Can't update ID property.", nameof(fieldsAndValuesToSet));
                }

                if (key.Equals("firstname", StringComparison.InvariantCultureIgnoreCase))
                {
                    var legacy = record.FirstName;
                    record.FirstName = value;
                    var validationResult = this.recordValidator.ValidateParameters(record);
                    if (!validationResult.Item1)
                    {
                        record.FirstName = legacy;
                        throw new ArgumentException(validationResult.Item2, nameof(fieldsAndValuesToSet));
                    }

                    continue;
                }

                if (key.Equals("lastname", StringComparison.InvariantCultureIgnoreCase))
                {
                    var legacy = record.LastName;
                    record.LastName = value;
                    var validationResult = this.recordValidator.ValidateParameters(record);
                    if (!validationResult.Item1)
                    {
                        record.LastName = legacy;
                        throw new ArgumentException(validationResult.Item2, nameof(fieldsAndValuesToSet));
                    }

                    continue;
                }

                if (key.Equals("dateofbirth", StringComparison.InvariantCultureIgnoreCase))
                {
                    var conversionResult = Converter.ConvertStringToDateTime(value);
                    if (!conversionResult.Item1)
                    {
                        throw new ArgumentException(conversionResult.Item2, nameof(fieldsAndValuesToSet));
                    }

                    var legacy = record.DateOfBirth;
                    record.DateOfBirth = conversionResult.Item3;
                    var validationResult = this.recordValidator.ValidateParameters(record);
                    if (!validationResult.Item1)
                    {
                        record.DateOfBirth = legacy;
                        throw new ArgumentException(validationResult.Item2, nameof(fieldsAndValuesToSet));
                    }

                    continue;
                }

                if (key.Equals("patronymicletter", StringComparison.InvariantCultureIgnoreCase))
                {
                    var conversionResult = Converter.ConvertStringToChar(value);
                    if (!conversionResult.Item1)
                    {
                        throw new ArgumentException(conversionResult.Item2, nameof(fieldsAndValuesToSet));
                    }

                    var legacy = record.PatronymicLetter;
                    record.PatronymicLetter = conversionResult.Item3;
                    var validationResult = this.recordValidator.ValidateParameters(record);
                    if (!validationResult.Item1)
                    {
                        record.PatronymicLetter = legacy;
                        throw new ArgumentException(validationResult.Item2, nameof(fieldsAndValuesToSet));
                    }

                    continue;
                }

                if (key.Equals("income", StringComparison.InvariantCultureIgnoreCase))
                {
                    var conversionResult = Converter.ConvertStringToDecimal(value);
                    if (!conversionResult.Item1)
                    {
                        throw new ArgumentException(conversionResult.Item2, nameof(fieldsAndValuesToSet));
                    }

                    var legacy = record.Income;
                    record.Income = conversionResult.Item3;
                    var validationResult = this.recordValidator.ValidateParameters(record);
                    if (!validationResult.Item1)
                    {
                        record.Income = legacy;
                        throw new ArgumentException(validationResult.Item2, nameof(fieldsAndValuesToSet));
                    }

                    continue;
                }

                if (key.Equals("height", StringComparison.InvariantCultureIgnoreCase))
                {
                    var conversionResult = Converter.ConvertStringToShort(value);
                    if (!conversionResult.Item1)
                    {
                        throw new ArgumentException(conversionResult.Item2, nameof(fieldsAndValuesToSet));
                    }

                    var legacy = record.Height;
                    record.Height = conversionResult.Item3;
                    var validationResult = this.recordValidator.ValidateParameters(record);
                    if (!validationResult.Item1)
                    {
                        record.Height = legacy;
                        throw new ArgumentException(validationResult.Item2, nameof(fieldsAndValuesToSet));
                    }

                    continue;
                }

                throw new ArgumentException("Key not exist.", nameof(fieldsAndValuesToSet));
            }
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

        private void FillDictionaries(FileCabinetRecord record)
        {
            if (this.firstNameDictionary.ContainsKey(record.FirstName))
            {
                this.firstNameDictionary[record.FirstName].Add(record);
            }
            else
            {
                this.firstNameDictionary.Add(record.FirstName, new List<FileCabinetRecord>());
                this.firstNameDictionary[record.FirstName].Add(record);
            }

            if (this.lastNameDictionary.ContainsKey(record.LastName))
            {
                this.lastNameDictionary[record.LastName].Add(record);
            }
            else
            {
                this.lastNameDictionary.Add(record.LastName, new List<FileCabinetRecord>());
                this.lastNameDictionary[record.LastName].Add(record);
            }

            if (this.dateOfBirthDictionary.ContainsKey(record.DateOfBirth))
            {
                this.dateOfBirthDictionary[record.DateOfBirth].Add(record);
            }
            else
            {
                this.dateOfBirthDictionary.Add(record.DateOfBirth, new List<FileCabinetRecord>());
                this.dateOfBirthDictionary[record.DateOfBirth].Add(record);
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
