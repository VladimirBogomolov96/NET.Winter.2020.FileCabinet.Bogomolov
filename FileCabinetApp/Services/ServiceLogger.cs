﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Provides methods to interact with records and writes info to log file.
    /// </summary>
    public sealed class ServiceLogger : IFileCabinetService, IDisposable
    {
        private IFileCabinetService service;
        private StreamWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLogger"/> class.
        /// </summary>
        /// <param name="service">Service to decorate.</param>
        public ServiceLogger(IFileCabinetService service)
        {
            this.service = service;
            this.writer = new StreamWriter("LogFile.log")
            {
                AutoFlush = true,
            };
        }

        /// <summary>
        /// Creates new records with given parameters and writes info to log file.
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

            this.writer.WriteLine($"{DateTime.Now} Calling CreateRecord() with FirstName = {transfer.FirstName}, LastName = {transfer.LastName}, DateOfBirth = {transfer.DateOfBirth}, Height = {transfer.Height}, Income = {transfer.Income}, PatrinymicLetter = {transfer.PatronymicLetter}.");
            var result = this.service.CreateRecord(transfer);
            this.writer.WriteLine($"{DateTime.Now} CreateRecord() returned {result}");
            return result;
        }

        /// <summary>
        /// Edits existing record and writes info to log file.
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

            this.writer.WriteLine($"{DateTime.Now} Calling EditRecord() with id = {id} FirstName = {transfer.FirstName}, LastName = {transfer.LastName}, DateOfBirth = {transfer.DateOfBirth}, Height = {transfer.Height}, Income = {transfer.Income}, PatrinymicLetter = {transfer.PatronymicLetter}.");
            this.service.EditRecord(id, transfer);
            this.writer.WriteLine($"{DateTime.Now} EditRecord() end execution.");
        }

        /// <summary>
        /// Finds all records with given date of birth and writes info to log file.
        /// </summary>
        /// <param name="dateOfBirth">Date of birth to match with.</param>
        /// <returns>Array of matching records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfbirth(DateTime dateOfBirth)
        {
            this.writer.WriteLine($"{DateTime.Now} Calling FindByDateOfbirth() with dateOfBirth = {dateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}.");
            var result = this.service.FindByDateOfbirth(dateOfBirth);
            this.writer.WriteLine($"{DateTime.Now} FindByDateOfbirth() return ReadOnlyCollection with {result.Count} FileCabinetRecords.");
            return result;
        }

        /// <summary>
        /// Finds all records with given first name and writes info to log file.
        /// </summary>
        /// <param name="firstName">First name to match with.</param>
        /// <returns>Array of matching records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            this.writer.WriteLine($"{DateTime.Now} Calling FindByFirstName with firstName = {firstName}.");
            var result = this.service.FindByFirstName(firstName);
            this.writer.WriteLine($"{DateTime.Now} FindByFirstName() return ReadOnlyCollection with {result.Count} FileCabinetRecords.");
            return result;
        }

        /// <summary>
        /// Finds all records with given last name and writes info to log file.
        /// </summary>
        /// <param name="lastName">Last name to match with.</param>
        /// <returns>Array of matching records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            this.writer.WriteLine($"{DateTime.Now} Calling FindByLastName with lastName = {lastName}.");
            var result = this.service.FindByLastName(lastName);
            this.writer.WriteLine($"{DateTime.Now} FindByLastName() return ReadOnlyCollection with {result.Count} FileCabinetRecords.");
            return result;
        }

        /// <summary>
        /// Gets all existing records and writes info to log file.
        /// </summary>
        /// <returns>Array of all existing records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            this.writer.WriteLine($"{DateTime.Now} Calling GetRecords().");
            var result = this.service.GetRecords();
            this.writer.WriteLine($"{DateTime.Now} GetRecords() return ReadOnlyCollection with {result.Count} FileCabinetRecords.");
            return result;
        }

        /// <summary>
        /// Counts amount of existing records and writes info to log file.
        /// </summary>
        /// <returns>Amount of existing records.</returns>
        public (int, int) GetStat()
        {
            this.writer.WriteLine($"{DateTime.Now} Calling GetStat().");
            var result = this.service.GetStat();
            this.writer.WriteLine($"{DateTime.Now} GetStat() return that service contains {result.Item1} items and {result.Item2} removed items.");
            return result;
        }

        /// <summary>
        /// Creates a snapshot of all records in current moment and writes info to log file.
        /// </summary>
        /// <returns>Snapshot of records.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            this.writer.WriteLine($"{DateTime.Now} Calling MakeSnapshot().");
            var result = this.service.MakeSnapshot();
            this.writer.WriteLine($"{DateTime.Now} MakeSnapshot() return snapshot of service in current moment.");
            return result;
        }

        /// <summary>
        /// Defragments file and writes info to log file.
        /// </summary>
        /// <returns>Amount of purged records.</returns>
        public int Purge()
        {
            this.writer.WriteLine($"{DateTime.Now} Calling Purge().");
            var result = this.service.Purge();
            this.writer.WriteLine($"{DateTime.Now} Purge() defragmrnt {result} records.");
            return result;
        }

        /// <summary>
        /// Removes record by given id and writes info to log file.
        /// </summary>
        /// <param name="id">ID of record to remove.</param>
        /// <returns>Whether record existed or not.</returns>
        public bool Remove(int id)
        {
            this.writer.WriteLine($"{DateTime.Now} Calling Purge() with id = {id}.");
            var result = this.service.Remove(id);
            this.writer.WriteLine($"{DateTime.Now} Remove() removing of record by id {id} was {result}.");
            return result;
        }

        /// <summary>
        /// Restores statement from snapshot and writes info to log file.
        /// </summary>
        /// <param name="snapshot">Snapshot that represent statement to restore.</param>
        /// <returns>Amount of new records added.</returns>
        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            this.writer.WriteLine($"{DateTime.Now} Calling Restore().");
            var result = this.service.Restore(snapshot);
            this.writer.WriteLine($"{DateTime.Now} Restore() imported {result} records.");
            return result;
        }

        /// <summary>
        /// Closes streamwriter.
        /// </summary>
        public void Dispose()
        {
            ((IDisposable)this.writer).Dispose();
        }

        /// <summary>
        /// Sets record validator.
        /// </summary>
        /// <param name="recordValidator">Rules of validation.</param>
        public void SetRecordValidator(IRecordValidator recordValidator)
        {
            this.service.SetRecordValidator(recordValidator);
        }
    }
}