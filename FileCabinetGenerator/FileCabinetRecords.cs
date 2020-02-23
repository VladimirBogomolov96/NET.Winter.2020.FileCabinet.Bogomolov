using System;
using System.Globalization;
using System.Xml.Serialization;

namespace FileCabinetGenerator
{
    [XmlRoot("records")]
    public class FileCabinetRecords
    {
        [XmlElement("record")]
        public FileCabinetRecordXml[] records;
    }
}
