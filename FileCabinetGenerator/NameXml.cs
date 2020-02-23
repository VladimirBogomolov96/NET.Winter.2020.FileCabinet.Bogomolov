using System;
using System.Globalization;
using System.Xml.Serialization;

namespace FileCabinetGenerator
{
    public class NameXml
    {
        [XmlAttribute("first")]
        /// <summary>
        /// Gets or sets first name of record subject.
        /// </summary>
        /// <value>Value of first name of a record.</value>
        public string FirstName { get; set; }

        [XmlAttribute("last")]
        /// <summary>
        /// Gets or sets last name of record subject.
        /// </summary>
        /// <value>Value of last name of a record.</value>
        public string LastName { get; set; }
    }
}
