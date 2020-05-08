using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides methods to save records into csv or xml.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private FileCabinetRecord[] records = Array.Empty<FileCabinetRecord>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        public FileCabinetServiceSnapshot()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">Records to save.</param>
        public FileCabinetServiceSnapshot(FileCabinetRecord[] records)
        {
            this.records = records;
        }

        /// <summary>
        /// Gets records as readonly collection.
        /// </summary>
        /// <value>Readonly collection of records.</value>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords
        {
            get
            {
                return Array.AsReadOnly(this.records);
            }
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
                throw new ArgumentNullException(nameof(writer), Configurator.GetConstantString("NullStream"));
            }

            using FileCabinetRecordCsvWriter csvWriter = new FileCabinetRecordCsvWriter(writer);
            foreach (var record in this.records)
            {
                csvWriter.Write(record);
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
                throw new ArgumentNullException(nameof(writer), Configurator.GetConstantString("NullStream"));
            }

            writer.WriteStartElement(Configurator.GetConstantString("XmlElementRecords"));
            using FileCabinetRecordXmlWriter xmlWriter = new FileCabinetRecordXmlWriter(writer);
            foreach (var record in this.records)
            {
                xmlWriter.Write(record);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets records from csv.
        /// </summary>
        /// <param name="streamReader">Stream reader to get records.</param>
        public void LoadFromCsv(StreamReader streamReader)
        {
            FileCabinetRecordCsvReader csvReader = new FileCabinetRecordCsvReader(streamReader);
            this.records = csvReader.ReadAll().ToArray();
        }

        /// <summary>
        /// Gets records from xml.
        /// </summary>
        /// <param name="xmlReader">Xml reader to get records.</param>
        public void LoadFromXml(XmlReader xmlReader)
        {
            FileCabinetRecordXmlReader fileXmlReader = new FileCabinetRecordXmlReader(xmlReader);
            try
            {
                this.records = fileXmlReader.ReadAll().ToArray();
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine(Configurator.GetConstantString("InvalidDeserialize"));
            }
        }
    }
}
