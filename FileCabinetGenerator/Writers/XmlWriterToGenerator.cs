using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Xml writer of file cabinet generator.
    /// </summary>
    public class XmlWriterToGenerator
    {
        private readonly XmlWriter fileStream;
        private readonly FileCabinetRecords records = new FileCabinetRecords();

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlWriterToGenerator"/> class.
        /// </summary>
        /// <param name="fileStream">File stream.</param>
        /// <param name="records">Records to serialize.</param>
        public XmlWriterToGenerator(XmlWriter fileStream, FileCabinetRecordXml[] records)
        {
            this.fileStream = fileStream;
            this.records.Records = records;
        }

        /// <summary>
        /// Writes records to xml.
        /// </summary>
        public void Write()
        {
            XmlSerializerNamespaces emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var xmlSerializer = new XmlSerializer(typeof(FileCabinetRecords));
            xmlSerializer.Serialize(this.fileStream, this.records, emptyNamespaces);
        }
    }
}
