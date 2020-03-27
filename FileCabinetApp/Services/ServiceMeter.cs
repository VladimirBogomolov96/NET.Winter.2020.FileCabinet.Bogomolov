using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Provides methods to interact with records and measure execution time.
    /// </summary>
    public class ServiceMeter : IFileCabinetService
    {
        private IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMeter"/> class.
        /// </summary>
        /// <param name="service">Service to decorate.</param>
        public ServiceMeter(IFileCabinetService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Creates new records with given parameters and measure execution time.
        /// </summary>
        /// <param name="transfer">Object to transfer parameters of new record.</param>
        /// <returns>ID of created record.</returns>
        /// <exception cref="ArgumentNullException">Throw when first name or last name is null, when transfer object is null.</exception>
        /// <exception cref="ArgumentException">Thrown when firs name or last name length is out of 2 and 60 chars or contains only whitespaces, when date of birth out of 01-Jan-1950 and current date, when height is out of 1 and 300 cm, when income is negative, when patronymic letter is not a latin uppercase letter.</exception>
        public int CreateRecord(RecordParametersTransfer transfer)
        {
            var stopWatch = Stopwatch.StartNew();
            var result = this.service.CreateRecord(transfer);
            Console.WriteLine($"CreateRecord method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>
        /// Edits existing record and measure execution time.
        /// </summary>
        /// <param name="id">ID of a record to edit.</param>
        /// <param name="transfer">Object to transfer new parameters to existing record.</param>
        /// <exception cref="ArgumentNullException">Throw when first name or last name is null, when transfer object is null.</exception>
        /// <exception cref="ArgumentException">Thrown when firs name or last name length is out of 2 and 60 chars or contains only whitespaces, when date of birth out of 01-Jan-1950 and current date, when height is out of 1 and 300 cm, when income is negative, when patronymic letter is not a latin uppercase letter.</exception>
        public void EditRecord(int id, RecordParametersTransfer transfer)
        {
            var stopWatch = Stopwatch.StartNew();
            this.service.EditRecord(id, transfer);
            Console.WriteLine($"EditRecord method execution duration is {stopWatch.ElapsedTicks} ticks.");
        }

        /// <summary>
        /// Gets all existing records and measure execution time.
        /// </summary>
        /// <returns>Array of all existing records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            var stopWatch = Stopwatch.StartNew();
            var result = this.service.GetRecords();
            Console.WriteLine($"GetRecords method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>
        /// Counts amount of existing records and measure execution time.
        /// </summary>
        /// <returns>Amount of existing records.</returns>
        public (int, int) GetStat()
        {
            var stopWatch = Stopwatch.StartNew();
            var result = this.service.GetStat();
            Console.WriteLine($"GetStat method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>
        /// Inserts new record and measure execution time.
        /// </summary>
        /// <param name="record">Record to insert.</param>
        /// <returns>Id of inserted record.</returns>
        public int Insert(FileCabinetRecord record)
        {
            var stopWatch = Stopwatch.StartNew();
            var result = this.service.Insert(record);
            Console.WriteLine($"Insert method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>
        /// Creates a snapshot of all records in current moment and measure execution time.
        /// </summary>
        /// <returns>Snapshot of records.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            var stopWatch = Stopwatch.StartNew();
            var result = this.service.MakeSnapshot();
            Console.WriteLine($"MakeSnapshot method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>
        /// Defragments file and measure execution time.
        /// </summary>
        /// <returns>Amount of purged records.</returns>
        public int Purge()
        {
            var stopWatch = Stopwatch.StartNew();
            var result = this.service.Purge();
            Console.WriteLine($"Purge method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>
        /// Deletes records and measure execution time.
        /// </summary>
        /// <param name="records">Records to delete.</param>
        /// <returns>IDs of deleted records.</returns>
        public IEnumerable<int> Delete(IEnumerable<FileCabinetRecord> records)
        {
            var stopWatch = Stopwatch.StartNew();
            var result = this.service.Delete(records);
            Console.WriteLine($"Delete method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>
        /// Removes record by given id and measure execution time.
        /// </summary>
        /// <param name="id">ID of record to remove.</param>
        /// <returns>Whether record existed or not.</returns>
        public bool Remove(int id)
        {
            var stopWatch = Stopwatch.StartNew();
            var result = this.service.Remove(id);
            Console.WriteLine($"Remove method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>
        /// Restores statement from snapshot and measure execution time.
        /// </summary>
        /// <param name="snapshot">Snapshot that represent statement to restore.</param>
        /// <returns>Amount of new records added.</returns>
        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            var stopWatch = Stopwatch.StartNew();
            var result = this.service.Restore(snapshot);
            Console.WriteLine($"Restore method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>
        /// Updates records and measure execution time.
        /// </summary>
        /// <param name="records">Records to update.</param>
        /// <param name="fieldsAndValuesToSet">Fields and values to set.</param>
        /// <returns>Amount of updated records.</returns>
        public int Update(IEnumerable<FileCabinetRecord> records, IEnumerable<IEnumerable<string>> fieldsAndValuesToSet)
        {
            var stopWatch = Stopwatch.StartNew();
            var result = this.service.Update(records, fieldsAndValuesToSet);
            Console.WriteLine($"Update method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>
        /// Sets record validator.
        /// </summary>
        /// <param name="recordValidator">Rules of validation.</param>
        public void SetRecordValidator(IRecordValidator recordValidator)
        {
            this.service.SetRecordValidator(recordValidator);
        }

        /// <summary>
        /// Gets cache and measure execution time.
        /// </summary>
        /// <returns>Cache.</returns>
        public Dictionary<string, string> GetCache()
        {
            var stopWatch = Stopwatch.StartNew();
            var result = this.service.GetCache();
            Console.WriteLine($"GetCache method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return result;
        }

        /// <summary>
        /// Saves condition and result of execution in cache and measure execution time.
        /// </summary>
        /// <param name="parameters">Parameters of execution.</param>
        /// <param name="result">Result of execution.</param>
        public void SaveInCache(string parameters, string result)
        {
            var stopWatch = Stopwatch.StartNew();
            this.service.SaveInCache(parameters, result);
            Console.WriteLine($"SaveInCache method execution duration is {stopWatch.ElapsedTicks} ticks.");
        }

        /// <summary>
        /// Clears cache and measure execution time.
        /// </summary>
        public void ClearCache()
        {
            var stopWatch = Stopwatch.StartNew();
            this.service.ClearCache();
            Console.WriteLine($"ClearCache method execution duration is {stopWatch.ElapsedTicks} ticks.");
        }
    }
}
