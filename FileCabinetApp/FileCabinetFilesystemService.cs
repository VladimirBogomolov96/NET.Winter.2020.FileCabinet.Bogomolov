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
        private const int SizeOfChar = 2;
        private const int SizeOfShort = 2;
        private const int SizeOfInt = 4;
        private const int SizeOfDecimal = 16;
        private const int SizeOfString = 120;
        private const int SizeOfRecord = 278;
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

        public void EditRecord(int id, RecordParametersTransfer transfer)
        {
            throw new NotImplementedException();
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

        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            throw new NotImplementedException();
        }

        public int GetStat()
        {
            throw new NotImplementedException();
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
