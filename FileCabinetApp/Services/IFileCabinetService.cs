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

        /// <summary>
        /// Deletes records.
        /// </summary>
        /// <param name="records">Records to delete.</param>
        /// <returns>IDs of deleted records.</returns>
        IEnumerable<int> Delete(IEnumerable<FileCabinetRecord> records);

        /// <summary>
        /// Updates records.
        /// </summary>
        /// <param name="records">Records to update.</param>
        /// <param name="fieldsAndValuesToSet">Fields and values to set.</param>
        /// <returns>Amount of updated records.</returns>
        int Update(IEnumerable<FileCabinetRecord> records, IEnumerable<IEnumerable<string>> fieldsAndValuesToSet);

        /// <summary>
        /// Gets cache.
        /// </summary>
        /// <returns>Cache.</returns>
        Dictionary<string, string> GetCache();

        /// <summary>
        /// Saves condition and result of execution in cache.
        /// </summary>
        /// <param name="parameters">Parameters of execution.</param>
        /// <param name="result">Result of execution.</param>
        void SaveInCache(string parameters, string result);

        /// <summary>
        /// Clears cache.
        /// </summary>
        void ClearCache();
    }
}