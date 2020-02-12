using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides methods to save records into csv or xml.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private readonly FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">Records to save.</param>
        public FileCabinetServiceSnapshot(FileCabinetRecord[] records)
        {
            this.records = records;
        }

        /// <summary>
        /// Saves records to csv file.
        /// </summary>
        /// <param name="writer">Streamwriter to save records.</param>
        /// <exception cref="ArgumentNullException">Writer must be not null.</exception>
        public void SaveToCsv(StreamWriter writer)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer), "Writer must be not null.");
            }

            using FileCabinetRecordCsvWriter csvWriter = new FileCabinetRecordCsvWriter(writer);
            for (int i = 0; i < this.records.Length; i++)
            {
                csvWriter.Write(this.records[i]);
            }
        }

        /// <summary>
        /// Saves records to xml file.
        /// </summary>
        /// <param name="writer">Streamwriter to save records.</param>
        /// <exception cref="ArgumentNullException">Writer must be not null.</exception>
        public void SaveToXml(XmlWriter writer)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer), "Writer must be not null.");
            }

            writer.WriteStartElement("records");
            using FileCabinetRecordXmlWriter xmlWriter = new FileCabinetRecordXmlWriter(writer);
            for (int i = 0; i < this.records.Length; i++)
            {
                xmlWriter.Write(this.records[i]);
            }

            writer.WriteEndElement();
        }
    }
}
