using System;
using System.Globalization;
using System.Xml.Serialization;
using FileCabinetApp;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Xml representation of record.
    /// </summary>
    [Serializable]
    public class FileCabinetRecordXml
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXml"/> class.
        /// </summary>
        public FileCabinetRecordXml()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXml"/> class.
        /// </summary>
        /// <param name="record">Records to represent to xml.</param>
        public FileCabinetRecordXml(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), Configurator.GetConstantString("NullRecord"));
            }

            this.Id = record.Id;
            this.Name = new NameXml() { FirstName = record.FirstName, LastName = record.LastName };
            this.DateOfBirth = record.DateOfBirth;
            this.Height = record.Height;
            this.Income = record.Income;
            this.PatronymicLetter = record.PatronymicLetter;
        }

        /// <summary>
        /// Gets or sets unique ID of a record.
        /// </summary>
        /// <value>Value of ID of a record.</value>
        [XmlAttribute("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets name.
        /// </summary>
        /// <value>Name of a record.</value>
        [XmlElement("name")]
        public NameXml Name { get; set; }

        /// <summary>
        /// Gets or sets date of birth of record subject.
        /// </summary>
        /// <value>Value of date of birth of a record.</value>
        [XmlIgnore]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets date of birth through string representation.
        /// </summary>
        /// <value>Value of date of birth of a record.</value>
        [XmlElement("dateOfBirth")]
        public string SomeDateString
        {
            get
            {
                return this.DateOfBirth.ToString(Configurator.GetConstantString("DateFormatDM"), CultureInfo.InvariantCulture);
            }

            set
            {
                bool isConverted = DateTime.TryParseExact(value, Configurator.GetConstantString("DateFormatMD"), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth);
                if (!isConverted)
                {
                    throw new ArgumentException(Configurator.GetConstantString("FailConversionStringToDate"), nameof(value));
                }

                this.DateOfBirth = dateOfBirth;
            }
        }

        /// <summary>
        /// Gets or sets height of record subject.
        /// </summary>
        /// <value>Value of height of a record.</value>
        [XmlElement("height")]
        public short Height { get; set; }

        /// <summary>
        /// Gets or sets income of record subject.
        /// </summary>
        /// <value>Value of income of a record.</value>
        [XmlElement("income")]
        public decimal Income { get; set; }

        /// <summary>
        /// Gets or sets patronymic letter of record subject.
        /// </summary>
        /// <value>Value of patronymic letter of a record.</value>
        [XmlIgnore]
        public char PatronymicLetter { get; set; }

        /// <summary>
        /// Gets or sets patronymic letter through string representation.
        /// </summary>
        /// <value>Patronymic letter through string representation.</value>
        [XmlElement("patronymicLetter")]
        public string PatronymicLetterAsString
        {
            get
            {
                return this.PatronymicLetter.ToString(CultureInfo.InvariantCulture);
            }

            set
            {
                this.PatronymicLetter = Convert.ToChar(value, CultureInfo.InvariantCulture);
            }
        }
    }
}
