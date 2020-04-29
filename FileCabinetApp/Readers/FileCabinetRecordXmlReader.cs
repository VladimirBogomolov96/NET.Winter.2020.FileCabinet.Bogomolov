using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Xml reader of filecabinet records.
    /// </summary>
    public class FileCabinetRecordXmlReader
    {
        private readonly XmlReader xmlReader;
        private FileCabinetRecordsXmlModel records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.
        /// </summary>
        /// <param name="xmlReader">Xml reader.</param>
        public FileCabinetRecordXmlReader(XmlReader xmlReader)
        {
            this.xmlReader = xmlReader;
        }

        /// <summary>
        /// Reads all strings from xml file.
        /// </summary>
        /// <returns>List of records.</returns>
        /// <exception cref="InvalidOperationException">Thrown when serializer can't deserialize data.</exception>
        /// <exception cref="InvalidCastException">Thrown when given data can't be casted to FileCabinetRecordsXmlModel.</exception>
        public IList<FileCabinetRecord> ReadAll()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(FileCabinetRecordsXmlModel));
            this.records = (FileCabinetRecordsXmlModel)xmlSerializer.Deserialize(this.xmlReader);
            return TransformFromXmlToBaseModel(new List<FileCabinetRecordXmlModel>(this.records.Records));
        }

        private static IList<FileCabinetRecord> TransformFromXmlToBaseModel(List<FileCabinetRecordXmlModel> source)
        {
            List<FileCabinetRecord> list = new List<FileCabinetRecord>();
            foreach (FileCabinetRecordXmlModel xmlModel in source)
            {
                try
                {
                    FileCabinetRecord record = new FileCabinetRecord
                    {
                        Id = xmlModel.Id,
                        FirstName = xmlModel.Name.FirstName,
                        LastName = xmlModel.Name.LastName,
                        PatronymicLetter = xmlModel.PatronymicLetter[0],
                        Income = xmlModel.Income,
                        Height = xmlModel.Height,
                        DateOfBirth = DateTime.ParseExact(xmlModel.DateOfBirth, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    };
                    list.Add(record);
                }
                catch (FormatException)
                {
                    Console.WriteLine($"Invalid data in {xmlModel.Id}{xmlModel.Name.FirstName}{xmlModel.Name.LastName}{xmlModel.PatronymicLetter[0]}{xmlModel.Income}{xmlModel.Height}. Data was skipped.");
                }
                catch (ArgumentException)
                {
                    Console.WriteLine($"Invalid data in {xmlModel.Id}{xmlModel.Name.FirstName}{xmlModel.Name.LastName}{xmlModel.PatronymicLetter[0]}{xmlModel.Income}{xmlModel.Height}. Data was skipped.");
                }
            }

            return list;
        }
    }
}
