using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Name xml model.
    /// </summary>
    public class FileCabinetRecordXmlName
    {
        /// <summary>
        /// Gets or sets first name.
        /// </summary>
        /// <value>First name.</value>
        [XmlAttribute("first")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets last name.
        /// </summary>
        /// <value>Last name.</value>
        [XmlAttribute("last")]
        public string LastName { get; set; }
    }
}
