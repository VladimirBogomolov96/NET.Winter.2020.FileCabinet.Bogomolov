using System;
using System.Globalization;
using System.Xml.Serialization;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Xml representation of records.
    /// </summary>
    [XmlRoot("records")]
    public class FileCabinetRecords
    {
        /// <summary>
        /// Gets or sets array of xml representations of records.
        /// </summary>
        /// <value>Array of xml representations of records.</value>
        [XmlElement("record")]
        public FileCabinetRecordXml[] Records { get; set; }
    }
}
