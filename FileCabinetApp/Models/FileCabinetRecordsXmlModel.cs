using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Model of filecabinet records.
    /// </summary>
    [XmlRoot("records")]
    public class FileCabinetRecordsXmlModel
    {
        /// <summary>
        /// Gets or sets array of records.
        /// </summary>
        /// <value>Array of records.</value>
        [XmlElement("record")]
#pragma warning disable CA1819 // Properties should not return arrays
        public FileCabinetRecordXmlModel[] Records { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays
    }
}
