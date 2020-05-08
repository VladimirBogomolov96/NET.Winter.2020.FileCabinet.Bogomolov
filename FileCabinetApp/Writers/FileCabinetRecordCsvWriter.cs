using System;
using System.Globalization;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides methods to write records into csv.
    /// </summary>
    public class FileCabinetRecordCsvWriter : IDisposable
    {
        private readonly TextWriter writer;
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
        /// </summary>
        /// <param name="writer">Streamwriter to save records.</param>
        public FileCabinetRecordCsvWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
        /// </summary>
        ~FileCabinetRecordCsvWriter()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Provides disposing of an object.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Writes a record into current stream.
        /// </summary>
        /// <param name="record">Record to write.</param>
        /// <exception cref="ArgumentNullException">Thrown when given record is null.</exception>
        public void Write(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), Configurator.GetConstantString("NullRecord"));
            }

            string writing = $"{record.Id},{record.FirstName},{record.PatronymicLetter},{record.LastName},{record.DateOfBirth.ToString(Configurator.GetConstantString("DateFormatMD"), CultureInfo.InvariantCulture)},{record.Height},{record.Income}";
            this.writer.WriteLine(writing);
        }

        /// <summary>
        /// Provides disposing of an object.
        /// </summary>
        /// <param name="disposing">If object is disposing or not.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.writer.Dispose();
                }

                this.disposed = true;
            }
        }
    }
}
