using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides methods to interact with records.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private readonly List<int> ids = new List<int>();
        private readonly List<string[]> cache = new List<string[]>();
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private IRecordValidator recordValidator;

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
        /// <exception cref="ArgumentNullException">Throw when transfer object is null.</exception>
        /// <exception cref="ArgumentException">Thrown when transfer data is invalid.</exception>
        public int CreateRecord(RecordParametersTransfer transfer)
        {
            if (transfer is null)
            {
                throw new ArgumentNullException(nameof(transfer), Configurator.GetConstantString("NullTransfer"));
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
            return record.Id;
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
        /// <exception cref="ArgumentNullException">Thrown when records is null.</exception>
        public IEnumerable<int> Delete(IEnumerable<FileCabinetRecord> records)
        {
            if (records is null)
            {
                throw new ArgumentNullException(nameof(records), Configurator.GetConstantString("NullRecordsSequence"));
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
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        /// <exception cref="ArgumentException">Thrown when records data is invalid.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when record with given id is already exists.</exception>
        public int Insert(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), Configurator.GetConstantString("NullRecord"));
            }

            if (!this.recordValidator.ValidateParameters(record).Item1)
            {
                throw new ArgumentException(this.recordValidator.ValidateParameters(record).Item2, nameof(record));
            }

            if (this.ids.Contains(record.Id))
            {
                throw new ArgumentOutOfRangeException(nameof(record), Configurator.GetConstantString("RecordIdExist"));
            }

            this.list.Add(record);
            this.ids.Add(record.Id);
            return record.Id;
        }

        /// <summary>
        /// Restores statement from snapshot.
        /// </summary>
        /// <param name="snapshot">Snapshot that represent statement to restore.</param>
        /// <returns>Amount of new records added.</returns>
        /// <exception cref="ArgumentNullException">Thrown when snapshot is null.</exception>
        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot is null)
            {
                throw new ArgumentNullException(nameof(snapshot), Configurator.GetConstantString("NullSnapshot"));
            }

            int count = 0;
            foreach (FileCabinetRecord record in snapshot.GetRecords)
            {
                var validationResult = this.recordValidator.ValidateParameters(record);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Invalid values in #{record.Id} record. {validationResult.Item2}");
                    continue;
                }

                if (this.ids.Contains(record.Id))
                {
                    var temp = this.list.Find(x => x.Id == record.Id);
                    temp.FirstName = record.FirstName;
                    temp.LastName = record.LastName;
                    temp.DateOfBirth = record.DateOfBirth;
                    temp.Height = record.Height;
                    temp.Income = record.Income;
                    temp.PatronymicLetter = record.PatronymicLetter;
                    count++;
                }
                else
                {
                    this.list.Add(record);
                    this.ids.Add(record.Id);
                    count++;
                }
            }

            return count;
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
        /// <exception cref="InvalidOperationException">Thrown always, because can't purge in memory service.</exception>
        public int Purge()
        {
            throw new InvalidOperationException(Configurator.GetConstantString("PurgeInMemory"));
        }

        /// <summary>
        /// Updates records.
        /// </summary>
        /// <param name="records">Records to update.</param>
        /// <param name="fieldsAndValuesToSet">Fields and values to set.</param>
        /// <returns>Amount of updated records.</returns>
        /// <exception cref="ArgumentNullException">Thrown when records or fields and values to set is null.</exception>
        /// <exception cref="ArgumentException">Thrown when data is invalid.</exception>
        public int Update(IEnumerable<FileCabinetRecord> records, IEnumerable<IEnumerable<string>> fieldsAndValuesToSet)
        {
            if (records is null)
            {
                throw new ArgumentNullException(nameof(records), Configurator.GetConstantString("NullRecordsSequence"));
            }

            if (fieldsAndValuesToSet is null)
            {
                throw new ArgumentNullException(nameof(fieldsAndValuesToSet), Configurator.GetConstantString("NullFieldsAndValues"));
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

                        try
                        {
                            this.UpdateRecord(record, fieldsAndValuesToSet);
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
                            throw new ArgumentException(ex.Message);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets cache.
        /// </summary>
        /// <param name="memoizationKey">Parameters of execution.</param>
        /// <returns>Cache.</returns>
        public string GetCache(string[] memoizationKey)
        {
            if (memoizationKey is null)
            {
                return null;
            }

            foreach (string[] arr in this.cache)
            {
                bool isFit = true;
                for (int i = 0; i < 8; i++)
                {
                    if (arr[i] != memoizationKey[i])
                    {
                        isFit = false;
                        break;
                    }
                }

                if (isFit)
                {
                    return arr[8];
                }
            }

            return null;
        }

        /// <summary>
        /// Saves condition and result of execution in cache.
        /// </summary>
        /// <param name="memoization">Parameters and result of execution.</param>
        public void SaveInCache(string[] memoization)
        {
            this.cache.Add(memoization);
        }

        /// <summary>
        /// Clears cache.
        /// </summary>
        public void ClearCache()
        {
            this.cache.Clear();
        }

        private void UpdateRecord(FileCabinetRecord record, IEnumerable<IEnumerable<string>> fieldsAndValuesToSet)
        {
            foreach (var keyValuePair in fieldsAndValuesToSet)
            {
                var key = keyValuePair.First();
                var value = keyValuePair.Last();
                if (key.Equals("id", StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new ArgumentException(Configurator.GetConstantString("IdChange"), nameof(fieldsAndValuesToSet));
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

                throw new ArgumentException(Configurator.GetConstantString("KeyNotExist"), nameof(fieldsAndValuesToSet));
            }
        }
    }
}
