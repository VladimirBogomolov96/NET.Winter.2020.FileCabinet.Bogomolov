using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        /// Creates new records with given parameters.
        /// </summary>
        /// <param name="transfer">Object to transfer parameters of new record.</param>
        /// <returns>ID of created record.</returns>
        /// <exception cref="ArgumentNullException">Thrown when transfer parameters is null.</exception>
        public int CreateRecord(RecordParametersTransfer transfer)
        {
            if (transfer is null)
            {
                throw new ArgumentNullException(nameof(transfer), "Parameters transfer must be not null.");
            }

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
            if (transfer is null)
            {
                throw new ArgumentNullException(nameof(transfer), "Parameters transfer must be not null.");
            }

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

        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfbirth(DateTime dateOfBirth)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            throw new NotImplementedException();
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

        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
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
    }
}
