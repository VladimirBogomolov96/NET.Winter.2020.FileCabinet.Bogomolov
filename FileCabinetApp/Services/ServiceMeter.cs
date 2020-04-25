using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FileCabinetApp.Services
{
    /// <summary>
    /// Provides methods to interact with records and measure execution time.
    /// </summary>
    public class ServiceMeter : IFileCabinetService
    {
        private readonly IFileCabinetService service;

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
        /// <exception cref="ArgumentNullException">Throw when transfer object is null.</exception>
        /// <exception cref="ArgumentException">Thrown when transfer data is invalid.</exception>
        public int CreateRecord(RecordParametersTransfer transfer)
        {
            var stopWatch = Stopwatch.StartNew();
            var result = this.service.CreateRecord(transfer);
            Console.WriteLine($"CreateRecord method execution duration is {stopWatch.ElapsedTicks} ticks.");
            return result;
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
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        /// <exception cref="ArgumentException">Thrown when records data is invalid or when record with given id is already exists.</exception>
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
        /// <exception cref="InvalidOperationException">Thrown when inner service is memory service.</exception>
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
        /// <exception cref="ArgumentNullException">Thrown when records is null.</exception>
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
        /// <exception cref="ArgumentNullException">Thrown when snapshot is null.</exception>
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
        /// <exception cref="ArgumentNullException">Thrown when records or fields and values to set is null.</exception>
        /// <exception cref="ArgumentException">Thrown when data is invalid.</exception>
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
