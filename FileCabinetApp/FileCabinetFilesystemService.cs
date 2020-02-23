using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
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
        private FileStream fileStream;
        private BinaryReader binaryReader;
        private BinaryWriter binaryWriter;
        private int lastId;
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
            this.recordValidator.ValidateParameters(transfer);
            this.lastId++;
            this.currentOffset += SizeOfShort;
            this.binaryWriter.Seek(this.currentOffset, 0);
            this.binaryWriter.Write(this.lastId);
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
            return this.lastId;
        }

        /// <summary>
        /// Edits existing record.
        /// </summary>
        /// <param name="id">ID of a record to edit.</param>
        /// <param name="transfer">Object to transfer new parameters to existing record.</param>
        /// <exception cref="ArgumentNullException">Thrown when transfer parameters is null.</exception>
        public void EditRecord(int id, RecordParametersTransfer transfer)
        {
            this.recordValidator.ValidateParameters(transfer);
            int tempOffset = ((id - 1) * SizeOfRecord) + SizeOfShort;
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
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfbirth(DateTime dateOfBirth)
        {
            int tempOffset = SizeOfShort + SizeOfInt + SizeOfString + SizeOfString;
            int tempID = 1;
            int fileLength = Convert.ToInt32(this.binaryReader.BaseStream.Length);
            List<FileCabinetRecord> fileCabinetRecords = new List<FileCabinetRecord>();
            while (tempOffset < fileLength)
            {
                DateTime tempDateOfBirth = new DateTime(1, 1, 1);
                this.binaryReader.BaseStream.Seek(tempOffset, 0);
                tempDateOfBirth = tempDateOfBirth.AddDays(this.binaryReader.ReadInt32() - 1);
                tempDateOfBirth = tempDateOfBirth.AddMonths(this.binaryReader.ReadInt32() - 1);
                tempDateOfBirth = tempDateOfBirth.AddYears(this.binaryReader.ReadInt32() - 1);

                if (tempDateOfBirth.Year == dateOfBirth.Year && tempDateOfBirth.Month == dateOfBirth.Month && tempDateOfBirth.Day == dateOfBirth.Day)
                {
                    fileCabinetRecords.Add(this.GetRecord(tempID));
                }

                tempID++;
                tempOffset += SizeOfRecord;
            }

            return fileCabinetRecords.AsReadOnly();
        }

        /// <summary>
        /// Finds all records with given first name.
        /// </summary>
        /// <param name="firstName">First name to match with.</param>
        /// <returns>Array of matching records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            int tempOffset = SizeOfShort + SizeOfInt;
            int tempID = 1;
            int fileLength = Convert.ToInt32(this.binaryReader.BaseStream.Length);
            List<FileCabinetRecord> fileCabinetRecords = new List<FileCabinetRecord>();
            string tempFirstName;
            while (tempOffset < fileLength)
            {
                this.binaryReader.BaseStream.Seek(tempOffset, 0);
                tempFirstName = this.binaryReader.ReadString();

                if (tempFirstName.Equals(firstName, StringComparison.InvariantCulture))
                {
                    fileCabinetRecords.Add(this.GetRecord(tempID));
                }

                tempID++;
                tempOffset += SizeOfRecord;
            }

            return fileCabinetRecords.AsReadOnly();
        }

        /// <summary>
        /// Finds all records with given last name.
        /// </summary>
        /// <param name="lastName">Last name to match with.</param>
        /// <returns>Array of matching records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            int tempOffset = SizeOfShort + SizeOfInt + SizeOfString;
            int tempID = 1;
            int fileLength = Convert.ToInt32(this.binaryReader.BaseStream.Length);
            List<FileCabinetRecord> fileCabinetRecords = new List<FileCabinetRecord>();
            string tempLastName;
            while (tempOffset < fileLength)
            {
                this.binaryReader.BaseStream.Seek(tempOffset, 0);
                tempLastName = this.binaryReader.ReadString();

                if (tempLastName.Equals(lastName, StringComparison.InvariantCulture))
                {
                    fileCabinetRecords.Add(this.GetRecord(tempID));
                }

                tempID++;
                tempOffset += SizeOfRecord;
            }

            return fileCabinetRecords.AsReadOnly();
        }

        /// <summary>
        /// Gets all existing records.
        /// </summary>
        /// <returns>Readonly collection of all existing records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            List<FileCabinetRecord> fileCabinetRecords = new List<FileCabinetRecord>();
            long tempOffset = 0;
            this.binaryReader.BaseStream.Seek(tempOffset, 0);
            while (this.binaryReader.BaseStream.Position < this.binaryReader.BaseStream.Length)
            {
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
        public int GetStat()
        {
            return Convert.ToInt32(this.binaryReader.BaseStream.Length) / SizeOfRecord;
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

        private FileCabinetRecord GetRecord(int id)
        {
            int tempOffset = ((id - 1) * SizeOfRecord) + SizeOfShort;
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
                        RecordParametersTransfer transfer = new RecordParametersTransfer(importData[importIndex].FirstName, importData[importIndex].LastName, importData[importIndex].DateOfBirth, importData[importIndex].Height, importData[importIndex].Income, importData[importIndex].PatronymicLetter);
                        this.recordValidator.ValidateParameters(transfer);
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

            this.lastId = resultRecords[^1].Id;
            this.WriteImportToFile(resultRecords);

            return importIndex;
        }

        public void SetRecordValidator(IRecordValidator recordValidator)
        {
            this.recordValidator = recordValidator;
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
    }
}
