using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

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
        private readonly Dictionary<string, List<int>> firstNameDictionary = new Dictionary<string, List<int>>();
        private readonly Dictionary<string, List<int>> lastNameDictionary = new Dictionary<string, List<int>>();
        private readonly Dictionary<DateTime, List<int>> dateOfBirthDictionary = new Dictionary<DateTime, List<int>>();
        private readonly Dictionary<int, int> idDictionary = new Dictionary<int, int>();
        private FileStream fileStream;
        private BinaryReader binaryReader;
        private BinaryWriter binaryWriter;
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
                throw new ArgumentNullException(nameof(fileStream), "Stream must not be null.");
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
        /// <exception cref="ArgumentNullException">Thrown when transfer parameters is null.</exception>
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

            int id = this.idDictionary.Keys.Max() + 1;
            this.FillDictionaries(transfer, id, this.currentOffset);
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
        public int Insert(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), "Record must be not null.");
            }

            if (!this.recordValidator.ValidateParameters(record).Item1)
            {
                throw new ArgumentException(this.recordValidator.ValidateParameters(record).Item2);
            }

            if (this.idDictionary.Keys.Contains(record.Id))
            {
                throw new ArgumentException("Record with given id already exists.", nameof(record));
            }

            this.FillDictionaries(record, this.currentOffset);
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
        /// Edits existing record.
        /// </summary>
        /// <param name="id">ID of a record to edit.</param>
        /// <param name="transfer">Object to transfer new parameters to existing record.</param>
        /// <exception cref="ArgumentNullException">Thrown when transfer parameters is null.</exception>
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

            int tempOffset = this.idDictionary[id];
            this.RemoveFromDictionaries(this.GetRecord(this.idDictionary[id]));
            this.binaryWriter.Seek(tempOffset, 0);
            if (this.binaryReader.ReadBoolean())
            {
                return;
            }

            this.FillDictionaries(transfer, id, tempOffset);
            tempOffset += SizeOfShort;
            this.binaryWriter.Seek(tempOffset, 0);
            this.binaryWriter.Write(id);
            tempOffset += SizeOfInt;
            this.binaryWriter.Write(transfer.FirstName);
            tempOffset += SizeOfString;
            this.binaryWriter.Seek(tempOffset, 0);
            this.binaryWriter.Write(transfer.LastName);
            tempOffset += SizeOfString;
            this.binaryWriter.Seek(tempOffset, 0);
            this.binaryWriter.Write(transfer.DateOfBirth.Day);
            this.binaryWriter.Write(transfer.DateOfBirth.Month);
            this.binaryWriter.Write(transfer.DateOfBirth.Year);
            this.binaryWriter.Write(transfer.PatronymicLetter);
            this.binaryWriter.Write(transfer.Income);
            this.binaryWriter.Write(transfer.Height);
        }

        /// <summary>
        /// Finds all records with given date of birth.
        /// </summary>
        /// <param name="dateOfBirth">Date of birth name to match with.</param>
        /// <returns>Array of matching records.</returns>
        public IEnumerable<FileCabinetRecord> FindByDateOfbirth(DateTime dateOfBirth)
        {
            if (this.dateOfBirthDictionary.TryGetValue(dateOfBirth, out List<int> offsets))
            {
                foreach (int offset in offsets)
                {
                    yield return this.GetRecord(offset);
                }
            }
        }

        /// <summary>
        /// Finds all records with given first name.
        /// </summary>
        /// <param name="firstName">First name to match with.</param>
        /// <returns>Array of matching records.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (this.firstNameDictionary.TryGetValue(firstName, out List<int> offsets))
            {
                foreach (int offset in offsets)
                {
                    yield return this.GetRecord(offset);
                }
            }
        }

        /// <summary>
        /// Finds all records with given last name.
        /// </summary>
        /// <param name="lastName">Last name to match with.</param>
        /// <returns>Array of matching records.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (this.lastNameDictionary.TryGetValue(lastName, out List<int> offsets))
            {
                foreach (int offset in offsets)
                {
                    yield return this.GetRecord(offset);
                }
            }
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
            ReadOnlyCollection<FileCabinetRecord> records = this.GetRecords();
            FileCabinetRecord[] fileCabinetRecordsArray = new FileCabinetRecord[records.Count];
            records.CopyTo(fileCabinetRecordsArray, 0);
            return new FileCabinetServiceSnapshot(fileCabinetRecordsArray);
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
        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot is null)
            {
                throw new ArgumentNullException(nameof(snapshot), "Snapshot must be not null.");
            }

            List<FileCabinetRecord> resultRecords = new List<FileCabinetRecord>();
            ReadOnlyCollection<FileCabinetRecord> importData = snapshot.GetRecords;
            ReadOnlyCollection<FileCabinetRecord> source = this.GetRecords();

            int sourceIndex = 0;
            int importIndex = 0;

            for (; sourceIndex < source.Count && importIndex < importData.Count;)
            {
                if (source[sourceIndex].Id < importData[importIndex].Id)
                {
                    resultRecords.Add(source[sourceIndex]);
                    sourceIndex++;
                }
                else if (source[sourceIndex].Id == importData[importIndex].Id)
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
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Wrong data in record #{0} : {1}", importData[importIndex].Id, ex.Message));
                    continue;
                }
            }

            for (; sourceIndex < source.Count; sourceIndex++)
            {
                resultRecords.Add(source[sourceIndex]);
            }

            this.WriteImportToFile(resultRecords);
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
            if (this.idDictionary.TryGetValue(id, out int tempOffset))
            {
                this.RemoveFromDictionaries(this.GetRecord(tempOffset));
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

        private void WriteImportToFile(List<FileCabinetRecord> records)
        {
            this.binaryWriter.BaseStream.Seek(0, 0);
            this.currentOffset = 0;
            foreach (var record in records)
            {
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
            }
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

        private void FillDictionaries(RecordParametersTransfer transfer, int id, int offset)
        {
            if (this.firstNameDictionary.ContainsKey(transfer.FirstName))
            {
                this.firstNameDictionary[transfer.FirstName].Add(offset);
            }
            else
            {
                this.firstNameDictionary.Add(transfer.FirstName, new List<int>());
                this.firstNameDictionary[transfer.FirstName].Add(offset);
            }

            if (this.lastNameDictionary.ContainsKey(transfer.LastName))
            {
                this.lastNameDictionary[transfer.LastName].Add(offset);
            }
            else
            {
                this.lastNameDictionary.Add(transfer.LastName, new List<int>());
                this.lastNameDictionary[transfer.LastName].Add(offset);
            }

            if (this.dateOfBirthDictionary.ContainsKey(transfer.DateOfBirth))
            {
                this.dateOfBirthDictionary[transfer.DateOfBirth].Add(offset);
            }
            else
            {
                this.dateOfBirthDictionary.Add(transfer.DateOfBirth, new List<int>());
                this.dateOfBirthDictionary[transfer.DateOfBirth].Add(offset);
            }

            this.idDictionary.Add(id, offset);
        }

        private void FillDictionaries(FileCabinetRecord record, int offset)
        {
            if (this.firstNameDictionary.ContainsKey(record.FirstName))
            {
                this.firstNameDictionary[record.FirstName].Add(offset);
            }
            else
            {
                this.firstNameDictionary.Add(record.FirstName, new List<int>());
                this.firstNameDictionary[record.FirstName].Add(offset);
            }

            if (this.lastNameDictionary.ContainsKey(record.LastName))
            {
                this.lastNameDictionary[record.LastName].Add(offset);
            }
            else
            {
                this.lastNameDictionary.Add(record.LastName, new List<int>());
                this.lastNameDictionary[record.LastName].Add(offset);
            }

            if (this.dateOfBirthDictionary.ContainsKey(record.DateOfBirth))
            {
                this.dateOfBirthDictionary[record.DateOfBirth].Add(offset);
            }
            else
            {
                this.dateOfBirthDictionary.Add(record.DateOfBirth, new List<int>());
                this.dateOfBirthDictionary[record.DateOfBirth].Add(offset);
            }

            this.idDictionary.Add(record.Id, offset);
        }

        private void RemoveFromDictionaries(FileCabinetRecord record)
        {
            this.firstNameDictionary[record.FirstName].Remove(this.idDictionary[record.Id]);
            this.lastNameDictionary[record.LastName].Remove(this.idDictionary[record.Id]);
            this.dateOfBirthDictionary[record.DateOfBirth].Remove(this.idDictionary[record.Id]);
            this.idDictionary.Remove(record.Id);
        }
    }
}
