using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides methods to interact with records.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// Creates new records with given parameters.
        /// </summary>
        /// <param name="transfer">Object to transfer parameters of new record.</param>
        /// <returns>ID of created record.</returns>
        int CreateRecord(RecordParametersTransfer transfer);

        /// <summary>
        /// Edits existing record.
        /// </summary>
        /// <param name="id">ID of a record to edit.</param>
        /// <param name="transfer">Object to transfer new parameters to existing record.</param>
        void EditRecord(int id, RecordParametersTransfer transfer);

        /// <summary>
        /// Sets record validator.
        /// </summary>
        /// <param name="recordValidator">Rules of validation.</param>
        void SetRecordValidator(IRecordValidator recordValidator);

        /// <summary>
        /// Creates a snapshot of all records in current moment.
        /// </summary>
        /// <returns>Snapshot of records.</returns>
        FileCabinetServiceSnapshot MakeSnapshot();

        /// <summary>
        /// Finds all records with given date of birth.
        /// </summary>
        /// <param name="dateOfBirth">Date of birth to match with.</param>
        /// <returns>Array of matching records.</returns>
        IEnumerable<FileCabinetRecord> FindByDateOfbirth(DateTime dateOfBirth);

        /// <summary>
        /// Finds all records with given first name.
        /// </summary>
        /// <param name="firstName">First name to match with.</param>
        /// <returns>Array of matching records.</returns>
        IEnumerable<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>
        /// Finds all records with given last name.
        /// </summary>
        /// <param name="lastName">Last name to match with.</param>
        /// <returns>Array of matching records.</returns>
        IEnumerable<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>
        /// Gets all existing records.
        /// </summary>
        /// <returns>Array of all existing records.</returns>
        ReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>
        /// Counts amount of existing records.
        /// </summary>
        /// <returns>Amount of existing and removed records.</returns>
        (int, int) GetStat();

        /// <summary>
        /// Restores statement from snapshot.
        /// </summary>
        /// <param name="snapshot">Snapshot that represent statement to restore.</param>
        /// <returns>Amount of new records added.</returns>
        int Restore(FileCabinetServiceSnapshot snapshot);

        /// <summary>
        /// Removes record by given id.
        /// </summary>
        /// <param name="id">ID of record to remove.</param>
        /// <returns>Whether records existed or not.</returns>
        bool Remove(int id);

        /// <summary>
        /// Defragments file.
        /// </summary>
        /// <returns>Amount of purged records.</returns>
        int Purge();

        /// <summary>
        /// Inserts new record.
        /// </summary>
        /// <param name="record">Record to insert.</param>
        /// <returns>Id of inserted record.</returns>
        int Insert(FileCabinetRecord record);
    }
}