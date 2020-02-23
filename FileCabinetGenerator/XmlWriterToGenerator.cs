using System;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using FileCabinetApp;

namespace FileCabinetGenerator
{
    public class XmlWriterToGenerator
    {
        private XmlWriter fileStream;
        private FileCabinetRecords records = new FileCabinetRecords();

        public XmlWriterToGenerator(XmlWriter fileStream, FileCabinetRecordXml[] records)
        {
            this.fileStream = fileStream;
            this.records.records = records;
        }

        public void Write()
        {
            XmlSerializerNamespaces emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var xmlSerializer = new XmlSerializer(typeof(FileCabinetRecords));
            xmlSerializer.Serialize(this.fileStream, this.records, emptyNamespaces);
        }
    }
}
