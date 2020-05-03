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
#pragma warning disable CA1819 // Properties should not return arrays
        public FileCabinetRecordXml[] Records { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays
    }
}
