using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Record xml model.
    /// </summary>
    [XmlRoot("record")]
    public class FileCabinetRecordXmlModel
    {
        /// <summary>
        /// Gets or sets ID.
        /// </summary>
        /// <value>ID.</value>
        [XmlAttribute("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets name.
        /// </summary>
        /// <value>Name.</value>
        [XmlElement("name")]
        public FileCabinetRecordXmlName Name { get; set; }

        /// <summary>
        /// Gets or sets date of birth.
        /// </summary>
        /// <value>Date of birth.</value>
        [XmlElement("dateOfBirth")]
        public string DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets patronymic letter.
        /// </summary>
        /// <value>Patronymic letter.</value>
        [XmlElement("patronymicLetter")]
        public string PatronymicLetter { get; set; }

        /// <summary>
        /// Gets or sets patronymic income.
        /// </summary>
        /// <value>Income.</value>
        [XmlElement("income")]
        public decimal Income { get; set; }

        /// <summary>
        /// Gets or sets patronymic height.
        /// </summary>
        /// <value>Height.</value>
        [XmlElement("height")]
        public short Height { get; set; }
    }
}
