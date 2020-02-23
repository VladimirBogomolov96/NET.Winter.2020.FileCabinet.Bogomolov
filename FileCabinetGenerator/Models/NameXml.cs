using System;
using System.Globalization;
using System.Xml.Serialization;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Name representation to xml.
    /// </summary>
    [Serializable]
    public class NameXml
    {
        /// <summary>
        /// Gets or sets first name of record subject.
        /// </summary>
        /// <value>Value of first name of a record.</value>
        [XmlAttribute("first")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets last name of record subject.
        /// </summary>
        /// <value>Value of last name of a record.</value>
        [XmlAttribute("last")]
        public string LastName { get; set; }
    }
}
