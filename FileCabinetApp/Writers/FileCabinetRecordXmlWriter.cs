using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides methods to write records into xml.
    /// </summary>
    public class FileCabinetRecordXmlWriter : IDisposable
    {
        private readonly XmlWriter writer;
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        /// <param name="writer">Streamwriter to save records.</param>
        public FileCabinetRecordXmlWriter(XmlWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        ~FileCabinetRecordXmlWriter()
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
        public void Write(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), "Record must be not null.");
            }

            this.writer.WriteStartElement("record");
            this.writer.WriteAttributeString("id", record.Id.ToString(CultureInfo.InvariantCulture));
            this.writer.WriteStartElement("name");
            this.writer.WriteAttributeString("first", record.FirstName);
            this.writer.WriteAttributeString("last", record.LastName);
            this.writer.WriteEndElement();
            this.writer.WriteStartElement("dateOfBirth");
            this.writer.WriteString(record.DateOfBirth.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
            this.writer.WriteEndElement();
            this.writer.WriteStartElement("height");
            this.writer.WriteString(record.Height.ToString(CultureInfo.InvariantCulture));
            this.writer.WriteEndElement();
            this.writer.WriteStartElement("income");
            this.writer.WriteString(record.Income.ToString(CultureInfo.InvariantCulture));
            this.writer.WriteEndElement();
            this.writer.WriteStartElement("patronymicLetter");
            this.writer.WriteString(record.PatronymicLetter.ToString(CultureInfo.InvariantCulture));
            this.writer.WriteEndElement();
            this.writer.WriteEndElement();
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
