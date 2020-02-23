using System;
using System.Globalization;
using System.Xml.Serialization;
using FileCabinetApp;

namespace FileCabinetGenerator
{
    [Serializable]
    public class FileCabinetRecordXml
    {
        public FileCabinetRecordXml()
        {
        }

        public FileCabinetRecordXml(FileCabinetRecord record)
        {
            this.Id = record.Id;
            this.Name = new NameXml() { FirstName = record.FirstName, LastName = record.LastName };
            this.DateOfBirth = record.DateOfBirth;
            this.Height = record.Height;
            this.Income = record.Income;
            this.PatronymicLetter = record.PatronymicLetter;
        }

        [XmlAttribute("id")]
        /// <summary>
        /// Gets or sets unique ID of a record.
        /// </summary>
        /// <value>Value of ID of a record.</value>
        public int Id { get; set; }

        [XmlElement("name")]
        /// <summary>
        /// Gets or sets name.
        /// </summary>
        /// <value>Name of a record.</value>
        public NameXml Name;

        [XmlIgnore]
        /// <summary>
        /// Gets or sets date of birth of record subject.
        /// </summary>
        /// <value>Value of date of birth of a record.</value>
        public DateTime DateOfBirth { get; set; }

        [XmlElement("DateOfBirth")]
        /// <summary>
        /// Gets or sets date of birth through string representation.
        /// </summary>
        /// <value>Value of date of birth of a record.</value>
        public string SomeDateString
        {
            get 
            { 
                return this.DateOfBirth.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            set 
            {
                bool isConverted = DateTime.TryParseExact(value, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth);
                if (!isConverted)
                {
                    throw new ArgumentException("Can't convert given string to date time.", nameof(value));
                }

                this.DateOfBirth = dateOfBirth;
            }
        }

        [XmlElement("height")]
        /// <summary>
        /// Gets or sets height of record subject.
        /// </summary>
        /// <value>Value of height of a record.</value>
        public short Height { get; set; }

        [XmlElement("income")]
        /// <summary>
        /// Gets or sets income of record subject.
        /// </summary>
        /// <value>Value of income of a record.</value>
        public decimal Income { get; set; }

        [XmlElement("patronymicLetter")]
        /// <summary>
        /// Gets or sets patronymic letter of record subject.
        /// </summary>
        /// <value>Value of patronymic letter of a record.</value>
        public char PatronymicLetter { get; set; }
    }
}
