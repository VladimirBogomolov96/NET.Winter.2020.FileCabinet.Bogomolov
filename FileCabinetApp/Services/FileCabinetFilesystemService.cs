﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace FileCabinetApp
{
    /// <summary>
    /// Works woth records using file system.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService, IDisposable
    {
        private const int SizeOfChar = 1;
        private const int SizeOfShort = 2;
        private const int SizeOfInt = 4;
        private const int SizeOfDecimal = 16;
        private const int SizeOfString = 122;
        private const int SizeOfRecord = 281;
        private readonly Dictionary<int, int> dictionaryIdOffset = new Dictionary<int, int>();
        private readonly Dictionary<string, string> cache = new Dictionary<string, string>();
        private readonly FileStream fileStream;
        private readonly BinaryReader binaryReader;
        private readonly BinaryWriter binaryWriter;
        private int currentOffset;
        private IRecordValidator recordValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="fileStream">Stream to work with file.</param>
        /// <exception cref="ArgumentNullException">Thrown when stream is null.</exception>
        public FileCabinetFilesystemService(FileStream fileStream)
        {
            if (fileStream is null)
            {
                throw new ArgumentNullException(nameof(fileStream), Configurator.GetConstantString("NullStream"));
            }

            this.fileStream = fileStream;
            this.binaryWriter = new BinaryWriter(fileStream);
            this.binaryReader = new BinaryReader(fileStream);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="fileStream">Stream to work with file.</param>
        /// <param name="recordValidator">Validation rules.</param>
        /// <exception cref="ArgumentNullException">Thrown when stream is null.</exception>
        public FileCabinetFilesystemService(FileStream fileStream, IRecordValidator recordValidator)
            : this(fileStream)
        {
            this.recordValidator = recordValidator;
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

            int id;
            if (this.dictionaryIdOffset.Count is 0)
            {
                id = 1;
            }
            else
            {
                id = this.dictionaryIdOffset.Keys.Max() + 1;
            }

            this.dictionaryIdOffset.Add(id, this.currentOffset);
            this.currentOffset += SizeOfShort;
            this.binaryWriter.Seek(this.currentOffset, 0);
            this.binaryWriter.Write(id);
            this.currentOffset += SizeOfInt;
            this.binaryWriter.Write(transfer.FirstName);
            this.currentOffset += SizeOfString;
            this.binaryWriter.Seek(this.currentOffset, 0);
            this.binaryWriter.Write(transfer.LastName);
            this.currentOffset += SizeOfString;
            this.binaryWriter.Seek(this.currentOffset, 0);
            this.binaryWriter.Write(transfer.DateOfBirth.Day);
            this.currentOffset += SizeOfInt;
            this.binaryWriter.Write(transfer.DateOfBirth.Month);
            this.currentOffset += SizeOfInt;
            this.binaryWriter.Write(transfer.DateOfBirth.Year);
            this.currentOffset += SizeOfInt;
            this.binaryWriter.Write(transfer.PatronymicLetter);
            this.currentOffset += SizeOfChar;
            this.binaryWriter.Write(transfer.Income);
            this.currentOffset += SizeOfDecimal;
            this.binaryWriter.Write(transfer.Height);
            this.currentOffset += SizeOfShort;
            return id;
        }

        /// <summary>
        /// Inserts new record.
        /// </summary>
        /// <param name="record">Record to insert.</param>
        /// <returns>Id of inserted record.</returns>
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        /// <exception cref="ArgumentException">Thrown when records data is invalid or when record with given id is already exists.</exception>
        public int Insert(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), Configurator.GetConstantString("NullRecord"));
            }

            if (!this.recordValidator.ValidateParameters(record).Item1)
            {
                throw new ArgumentException(this.recordValidator.ValidateParameters(record).Item2);
            }

            if (this.dictionaryIdOffset.Keys.Contains(record.Id))
            {
                throw new ArgumentException(Configurator.GetConstantString("RecordIdExist"), nameof(record));
            }

            this.dictionaryIdOffset.Add(record.Id, this.currentOffset);
            this.currentOffset += SizeOfShort;
            this.binaryWriter.Seek(this.currentOffset, 0);
            this.binaryWriter.Write(record.Id);
            this.currentOffset += SizeOfInt;
            this.binaryWriter.Write(record.FirstName);
            this.currentOffset += SizeOfString;
            this.binaryWriter.Seek(this.currentOffset, 0);
            this.binaryWriter.Write(record.LastName);
            this.currentOffset += SizeOfString;
            this.binaryWriter.Seek(this.currentOffset, 0);
            this.binaryWriter.Write(record.DateOfBirth.Day);
            this.currentOffset += SizeOfInt;
            this.binaryWriter.Write(record.DateOfBirth.Month);
            this.currentOffset += SizeOfInt;
            this.binaryWriter.Write(record.DateOfBirth.Year);
            this.currentOffset += SizeOfInt;
            this.binaryWriter.Write(record.PatronymicLetter);
            this.currentOffset += SizeOfChar;
            this.binaryWriter.Write(record.Income);
            this.currentOffset += SizeOfDecimal;
            this.binaryWriter.Write(record.Height);
            this.currentOffset += SizeOfShort;
            return record.Id;
        }

        /// <summary>
        /// Gets all existing records.
        /// </summary>
        /// <returns>Readonly collection of all existing records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            List<FileCabinetRecord> fileCabinetRecords = new List<FileCabinetRecord>();
            long tempOffset = 0;
            while (tempOffset < this.binaryReader.BaseStream.Length)
            {
                this.binaryReader.BaseStream.Seek(tempOffset, 0);
                if (this.binaryReader.ReadBoolean())
                {
                    tempOffset += SizeOfRecord;
                    continue;
                }

                FileCabinetRecord newRecord = new FileCabinetRecord();
                tempOffset += SizeOfShort;
                this.binaryReader.BaseStream.Seek(tempOffset, 0);
                newRecord.Id = this.binaryReader.ReadInt32();
                tempOffset += SizeOfInt;
                newRecord.FirstName = this.binaryReader.ReadString();
                this.binaryReader.BaseStream.Seek(tempOffset + SizeOfString, 0);
                newRecord.LastName = this.binaryReader.ReadString();
                tempOffset += SizeOfString;
                this.binaryReader.BaseStream.Seek(tempOffset + SizeOfString, 0);
                int day = this.binaryReader.ReadInt32();
                int month = this.binaryReader.ReadInt32();
                int year = this.binaryReader.ReadInt32();
                newRecord.DateOfBirth = newRecord.DateOfBirth.AddDays(day - 1);
                newRecord.DateOfBirth = newRecord.DateOfBirth.AddMonths(month - 1);
                newRecord.DateOfBirth = newRecord.DateOfBirth.AddYears(year - 1);
                newRecord.PatronymicLetter = this.binaryReader.ReadChar();
                newRecord.Income = this.binaryReader.ReadDecimal();
                newRecord.Height = this.binaryReader.ReadInt16();
                tempOffset = this.binaryReader.BaseStream.Position;
                fileCabinetRecords.Add(newRecord);
            }

            return fileCabinetRecords.AsReadOnly();
        }

        /// <summary>
        /// Counts amount of existing records.
        /// </summary>
        /// <returns>Amount of existing records.</returns>
        public (int, int) GetStat()
        {
            int tempOffset = 0;
            int counter = 0;
            int removedCounter = 0;
            int fileLength = Convert.ToInt32(this.binaryReader.BaseStream.Length);
            while (tempOffset < fileLength)
            {
                this.binaryReader.BaseStream.Seek(tempOffset, 0);
                if (this.binaryReader.ReadBoolean())
                {
                    tempOffset += SizeOfRecord;
                    removedCounter++;
                    continue;
                }

                counter++;
                tempOffset += SizeOfRecord;
            }

            return (counter, removedCounter);
        }

        /// <summary>
        /// Creates a snapshot of all records in current moment.
        /// </summary>
        /// <returns>Snapshot of records.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.GetRecords().ToArray());
        }

        /// <summary>
        /// Dispose service.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
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

                if (this.dictionaryIdOffset.ContainsKey(record.Id))
                {
                    this.WriteToFile(record, this.dictionaryIdOffset[record.Id]);
                    count++;
                }
                else
                {
                    this.dictionaryIdOffset.Add(record.Id, this.currentOffset);
                    this.WriteToFile(record, this.currentOffset);
                    this.currentOffset += SizeOfRecord;
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
        /// Deletes records.
        /// </summary>
        /// <param name="records">Records to delete.</param>
        /// <returns>IDs of deleted records.</returns>
        /// <exception cref="ArgumentNullException">Thrown when recors is null.</exception>
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
            if (this.dictionaryIdOffset.TryGetValue(id, out int tempOffset))
            {
                this.dictionaryIdOffset.Remove(id);
                this.binaryWriter.Seek(tempOffset, 0);
                this.binaryWriter.Write(true);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Defragments file.
        /// </summary>
        /// <returns>Amount of purged records.</returns>
        public int Purge()
        {
            int tempOffset = 0;
            long initialLength = this.binaryReader.BaseStream.Length;
            int deletePosition;
            while (tempOffset < this.binaryReader.BaseStream.Length)
            {
                this.binaryReader.BaseStream.Seek(tempOffset, 0);
                if (this.binaryReader.ReadBoolean())
                {
                    deletePosition = tempOffset;
                    tempOffset += SizeOfRecord;
                    while (tempOffset < this.binaryReader.BaseStream.Length)
                    {
                        this.binaryReader.BaseStream.Seek(tempOffset, 0);
                        if (!this.binaryReader.ReadBoolean())
                        {
                            var record = this.GetRecord(tempOffset);
                            this.binaryWriter.BaseStream.Seek(tempOffset, 0);
                            this.binaryWriter.Write(true);
                            this.WriteToFile(record, deletePosition);
                            tempOffset = deletePosition;
                            break;
                        }

                        tempOffset += SizeOfRecord;
                    }

                    if (tempOffset >= this.binaryReader.BaseStream.Length)
                    {
                        this.binaryReader.BaseStream.SetLength(deletePosition);
                        this.currentOffset = deletePosition;
                    }
                }

                tempOffset += SizeOfRecord;
            }

            return (int)(initialLength - this.binaryReader.BaseStream.Length) / SizeOfRecord;
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
                int tempOffset = this.dictionaryIdOffset[record.Id];
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
                    this.WriteToFile(record, this.dictionaryIdOffset[record.Id]);
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
                    this.WriteToFile(record, this.dictionaryIdOffset[record.Id]);
                    throw new ArgumentException(ex.Message);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets cache.
        /// </summary>
        /// <returns>Cache.</returns>
        public Dictionary<string, string> GetCache()
        {
            return this.cache;
        }

        /// <summary>
        /// Saves condition and result of execution in cache.
        /// </summary>
        /// <param name="parameters">Parameters of execution.</param>
        /// <param name="result">Result of execution.</param>
        public void SaveInCache(string parameters, string result)
        {
            this.cache.Add(parameters, result);
        }

        /// <summary>
        /// Clears cache.
        /// </summary>
        public void ClearCache()
        {
            this.cache.Clear();
        }

        /// <summary>
        /// If service is disposing, close streams.
        /// </summary>
        /// <param name="disposing">If service is disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.binaryWriter.Dispose();
                this.binaryReader.Dispose();
                this.fileStream.Dispose();
            }
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

        private void WriteToFile(FileCabinetRecord record, int offset)
        {
            this.binaryWriter.Seek(offset, 0);
            this.binaryWriter.Write(false);
            offset += SizeOfShort;
            this.binaryWriter.Seek(offset, 0);
            this.binaryWriter.Write(record.Id);
            offset += SizeOfInt;
            this.binaryWriter.Write(record.FirstName);
            offset += SizeOfString;
            this.binaryWriter.Seek(offset, 0);
            this.binaryWriter.Write(record.LastName);
            offset += SizeOfString;
            this.binaryWriter.Seek(offset, 0);
            this.binaryWriter.Write(record.DateOfBirth.Day);
            this.binaryWriter.Write(record.DateOfBirth.Month);
            this.binaryWriter.Write(record.DateOfBirth.Year);
            this.binaryWriter.Write(record.PatronymicLetter);
            this.binaryWriter.Write(record.Income);
            this.binaryWriter.Write(record.Height);
        }

        private FileCabinetRecord GetRecord(int offset)
        {
            int tempOffset = offset + SizeOfShort;
            this.binaryReader.BaseStream.Seek(tempOffset, 0);
            FileCabinetRecord tempRecord = new FileCabinetRecord
            {
                Id = this.binaryReader.ReadInt32(),
            };
            tempOffset += SizeOfInt;
            tempRecord.FirstName = this.binaryReader.ReadString();
            this.binaryReader.BaseStream.Seek(tempOffset + SizeOfString, 0);
            tempRecord.LastName = this.binaryReader.ReadString();
            tempOffset += SizeOfString;
            this.binaryReader.BaseStream.Seek(tempOffset + SizeOfString, 0);
            int day = this.binaryReader.ReadInt32();
            int month = this.binaryReader.ReadInt32();
            int year = this.binaryReader.ReadInt32();
            tempRecord.DateOfBirth = tempRecord.DateOfBirth.AddDays(day - 1);
            tempRecord.DateOfBirth = tempRecord.DateOfBirth.AddMonths(month - 1);
            tempRecord.DateOfBirth = tempRecord.DateOfBirth.AddYears(year - 1);
            tempRecord.PatronymicLetter = this.binaryReader.ReadChar();
            tempRecord.Income = this.binaryReader.ReadDecimal();
            tempRecord.Height = this.binaryReader.ReadInt16();

            return tempRecord;
        }
    }
}
