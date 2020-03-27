using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Command handler to select method.
    /// </summary>
    public class SelectCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCommandHandler"/> class.
        /// </summary>
        /// <param name="service">File cabinet service to call.</param>
        public SelectCommandHandler(IFileCabinetService service)
            : base(service)
        {
        }

        /// <summary>
        /// Handles command line request.
        /// </summary>
        /// <param name="commandRequest">Command line request.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                Console.WriteLine("Wrong command line parameter.");
                return;
            }

            if (commandRequest.Command is null)
            {
                Console.WriteLine("Wrong command line parameter.");
                return;
            }

            if (commandRequest.Command.Equals("select", StringComparison.InvariantCultureIgnoreCase))
            {
                if (this.Service.GetCache().TryGetValue(commandRequest.Parameters.Replace(" ", string.Empty, StringComparison.InvariantCultureIgnoreCase), out string resultFromCache))
                {
                    Console.WriteLine(resultFromCache);
                }
                else
                {
                    try
                    {
                        string result = this.Select(commandRequest.Parameters);
                        this.Service.SaveInCache(commandRequest.Parameters.Replace(" ", string.Empty, StringComparison.InvariantCultureIgnoreCase), result);
                        Console.WriteLine(result);
                    }
                    catch (ArgumentException)
                    {
                        Console.WriteLine("Wrong parameters.");
                    }
                }
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private string Select(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters), "Parameters must be not null.");
            }

            var temp = parameters.Split("where");
            var fieldsToShow = temp[0].Split(',');
            for (int i = 0; i < fieldsToShow.Length; i++)
            {
                fieldsToShow[i] = fieldsToShow[i].Trim();
            }

            IEnumerable<FileCabinetRecord> recordsToShow;
            Func<List<int>, List<string>, List<string>, List<DateTime>, List<char>, List<decimal>, List<short>, IEnumerable<FileCabinetRecord>> func;
            IEnumerable<string> searchParams;
            if (temp.Length == 1)
            {
                recordsToShow = this.Service.GetRecords();
            }
            else if (temp.Length > 2)
            {
                throw new ArgumentException("Wrong parameters.", nameof(parameters));
            }
            else
            {
                var orSearchParams = temp[1].Split("or");
                var andSearchParams = temp[1].Split("and");
                bool isSamePropertiesPossible;

                if (orSearchParams.Length > andSearchParams.Length)
                {
                    func = this.SelectOr;
                    searchParams = orSearchParams;
                    isSamePropertiesPossible = true;
                }
                else
                {
                    func = this.SelectAnd;
                    searchParams = andSearchParams;
                    isSamePropertiesPossible = false;
                }

                if (searchParams.Count() < 2)
                {
                    throw new ArgumentException("Must be at least 2 search conditions.", nameof(parameters));
                }

                IEnumerable<IEnumerable<string>> fieldsAndValuesToFind = searchParams.Select(x => x.Split('=').Select(y => y.Trim()));
                List<int> searchRecordId = new List<int>();
                List<string> searchRecordFirstName = new List<string>();
                List<string> searchRecordLastName = new List<string>();
                List<DateTime> searchRecordDateOfBirth = new List<DateTime>();
                List<char> searchRecordPatronymicLetter = new List<char>();
                List<decimal> searchRecordIncome = new List<decimal>();
                List<short> searchRecordHeight = new List<short>();
                if (isSamePropertiesPossible)
                {
                    foreach (var pair in fieldsAndValuesToFind)
                    {
                        this.SetSearchParametersOr(searchRecordId, searchRecordFirstName, searchRecordLastName, searchRecordDateOfBirth, searchRecordPatronymicLetter, searchRecordIncome, searchRecordHeight, pair.First(), pair.Last());
                    }
                }
                else
                {
                    foreach (var pair in fieldsAndValuesToFind)
                    {
                        this.SetSearchParametersAnd(searchRecordId, searchRecordFirstName, searchRecordLastName, searchRecordDateOfBirth, searchRecordPatronymicLetter, searchRecordIncome, searchRecordHeight, pair.First(), pair.Last());
                    }
                }

                recordsToShow = func.Invoke(searchRecordId, searchRecordFirstName, searchRecordLastName, searchRecordDateOfBirth, searchRecordPatronymicLetter, searchRecordIncome, searchRecordHeight);
            }

            return this.CreateTable(recordsToShow, fieldsToShow);
        }

        private IEnumerable<FileCabinetRecord> SelectAnd(List<int> searchRecordId, List<string> searchRecordFirstName, List<string> searchRecordLastName, List<DateTime> searchRecordDateOfBirth, List<char> searchRecordPatronymicLetter, List<decimal> searchRecordIncome, List<short> searchRecordHeight)
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>(this.Service.GetRecords());
            if (searchRecordId.Count == 1)
            {
                records.RemoveAll(x => x.Id != searchRecordId[0]);
            }

            if (searchRecordFirstName.Count == 1)
            {
                records.RemoveAll(x => x.FirstName != searchRecordFirstName[0]);
            }

            if (searchRecordLastName.Count == 1)
            {
                records.RemoveAll(x => x.LastName != searchRecordLastName[0]);
            }

            if (searchRecordDateOfBirth.Count == 1)
            {
                records.RemoveAll(x => (x.DateOfBirth.Year != searchRecordDateOfBirth[0].Year) || (x.DateOfBirth.Month != searchRecordDateOfBirth[0].Month) || (x.DateOfBirth.Day != searchRecordDateOfBirth[0].Day));
            }

            if (searchRecordPatronymicLetter.Count == 1)
            {
                records.RemoveAll(x => x.PatronymicLetter != searchRecordPatronymicLetter[0]);
            }

            if (searchRecordIncome.Count == 1)
            {
                records.RemoveAll(x => x.Income != searchRecordIncome[0]);
            }

            if (searchRecordHeight.Count == 1)
            {
                records.RemoveAll(x => x.Height != searchRecordHeight[0]);
            }

            return records;
        }

        private IEnumerable<FileCabinetRecord> SelectOr(List<int> searchRecordId, List<string> searchRecordFirstName, List<string> searchRecordLastName, List<DateTime> searchRecordDateOfBirth, List<char> searchRecordPatronymicLetter, List<decimal> searchRecordIncome, List<short> searchRecordHeight)
        {
            List<FileCabinetRecord> result = new List<FileCabinetRecord>();
            IEnumerable<FileCabinetRecord> records = this.Service.GetRecords();
            if (searchRecordId.Count > 0)
            {
                foreach (int id in searchRecordId)
                {
                    result.AddRange(records.Where(x => x.Id == id));
                }
            }

            if (searchRecordFirstName.Count > 0)
            {
                foreach (string firstName in searchRecordFirstName)
                {
                    result.AddRange(records.Where(x => x.FirstName == firstName));
                }
            }

            if (searchRecordLastName.Count > 0)
            {
                foreach (string lastName in searchRecordLastName)
                {
                    result.AddRange(records.Where(x => x.LastName == lastName));
                }
            }

            if (searchRecordDateOfBirth.Count > 0)
            {
                foreach (DateTime dateOfBirth in searchRecordDateOfBirth)
                {
                    result.AddRange(records.Where(x => x.DateOfBirth.Year == dateOfBirth.Year && x.DateOfBirth.Month == dateOfBirth.Month && x.DateOfBirth.Day == dateOfBirth.Day));
                }
            }

            if (searchRecordPatronymicLetter.Count > 0)
            {
                foreach (char patronymicLetter in searchRecordPatronymicLetter)
                {
                    result.AddRange(records.Where(x => x.PatronymicLetter == patronymicLetter));
                }
            }

            if (searchRecordIncome.Count > 0)
            {
                foreach (decimal income in searchRecordIncome)
                {
                    result.AddRange(records.Where(x => x.Income == income));
                }
            }

            if (searchRecordHeight.Count > 0)
            {
                foreach (short height in searchRecordHeight)
                {
                    result.AddRange(records.Where(x => x.Height == height));
                }
            }

            return result.Distinct();
        }

        private void SetSearchParametersAnd(List<int> searchRecordId, List<string> searchRecordFirstName, List<string> searchRecordLastName, List<DateTime> searchRecordDateOfBirth, List<char> searchRecordPatronymicLetter, List<decimal> searchRecordIncome, List<short> searchRecordHeight, string key, string value)
        {
            if (key.Equals("id", StringComparison.InvariantCultureIgnoreCase))
            {
                if (searchRecordId.Count > 0)
                {
                    throw new ArgumentException("Set one parameter to fint to one property.", nameof(key));
                }

                var conversionResultOfId = Converter.ConvertStringToInt(value);
                if (!conversionResultOfId.Item1)
                {
                    throw new ArgumentException(conversionResultOfId.Item2, nameof(value));
                }

                searchRecordId.Add(conversionResultOfId.Item3);
                return;
            }

            if (key.Equals("firstname", StringComparison.InvariantCultureIgnoreCase))
            {
                if (searchRecordFirstName.Count > 0)
                {
                    throw new ArgumentException("Set one parameter to fint to one property.", nameof(key));
                }

                searchRecordFirstName.Add(value);
                return;
            }

            if (key.Equals("lastname", StringComparison.InvariantCultureIgnoreCase))
            {
                if (searchRecordLastName.Count > 0)
                {
                    throw new ArgumentException("Set one parameter to fint to one property.", nameof(key));
                }

                searchRecordLastName.Add(value);
                return;
            }

            if (key.Equals("dateofbirth", StringComparison.InvariantCultureIgnoreCase))
            {
                if (searchRecordDateOfBirth.Count > 0)
                {
                    throw new ArgumentException("Set one parameter to fint to one property.", nameof(key));
                }

                var dateOfBirthConversionResult = Converter.ConvertStringToDateTime(value);
                if (!dateOfBirthConversionResult.Item1)
                {
                    throw new ArgumentException(dateOfBirthConversionResult.Item2, nameof(value));
                }

                searchRecordDateOfBirth.Add(dateOfBirthConversionResult.Item3);
                return;
            }

            if (key.Equals("patronymicletter", StringComparison.InvariantCultureIgnoreCase))
            {
                if (searchRecordPatronymicLetter.Count > 0)
                {
                    throw new ArgumentException("Set one parameter to fint to one property.", nameof(key));
                }

                var patronymicLetterConversionResult = Converter.ConvertStringToChar(value);
                if (!patronymicLetterConversionResult.Item1)
                {
                    throw new ArgumentException(patronymicLetterConversionResult.Item2, nameof(value));
                }

                searchRecordPatronymicLetter.Add(patronymicLetterConversionResult.Item3);
                return;
            }

            if (key.Equals("income", StringComparison.InvariantCultureIgnoreCase))
            {
                if (searchRecordIncome.Count > 0)
                {
                    throw new ArgumentException("Set one parameter to fint to one property.", nameof(key));
                }

                var incomeConversionResult = Converter.ConvertStringToDecimal(value);
                if (!incomeConversionResult.Item1)
                {
                    throw new ArgumentException(incomeConversionResult.Item2, nameof(value));
                }

                searchRecordIncome.Add(incomeConversionResult.Item3);
                return;
            }

            if (key.Equals("height", StringComparison.InvariantCultureIgnoreCase))
            {
                if (searchRecordHeight.Count > 0)
                {
                    throw new ArgumentException("Set one parameter to fint to one property.", nameof(key));
                }

                var heightConversionResult = Converter.ConvertStringToShort(value);
                if (!heightConversionResult.Item1)
                {
                    throw new ArgumentException(heightConversionResult.Item2, nameof(value));
                }

                searchRecordHeight.Add(heightConversionResult.Item3);
                return;
            }

            throw new ArgumentException("Wrong property name.", nameof(key));
        }

        private void SetSearchParametersOr(List<int> searchRecordId, List<string> searchRecordFirstName, List<string> searchRecordLastName, List<DateTime> searchRecordDateOfBirth, List<char> searchRecordPatronymicLetter, List<decimal> searchRecordIncome, List<short> searchRecordHeight, string key, string value)
        {
            if (key.Equals("id", StringComparison.InvariantCultureIgnoreCase))
            {
                var conversionResultOfId = Converter.ConvertStringToInt(value);
                if (!conversionResultOfId.Item1)
                {
                    throw new ArgumentException(conversionResultOfId.Item2, nameof(value));
                }

                searchRecordId.Add(conversionResultOfId.Item3);
                return;
            }

            if (key.Equals("firstname", StringComparison.InvariantCultureIgnoreCase))
            {
                searchRecordFirstName.Add(value);
                return;
            }

            if (key.Equals("lastname", StringComparison.InvariantCultureIgnoreCase))
            {
                searchRecordLastName.Add(value);
                return;
            }

            if (key.Equals("dateofbirth", StringComparison.InvariantCultureIgnoreCase))
            {
                var dateOfBirthConversionResult = Converter.ConvertStringToDateTime(value);
                if (!dateOfBirthConversionResult.Item1)
                {
                    throw new ArgumentException(dateOfBirthConversionResult.Item2, nameof(value));
                }

                searchRecordDateOfBirth.Add(dateOfBirthConversionResult.Item3);
                return;
            }

            if (key.Equals("patronymicletter", StringComparison.InvariantCultureIgnoreCase))
            {
                var patronymicLetterConversionResult = Converter.ConvertStringToChar(value);
                if (!patronymicLetterConversionResult.Item1)
                {
                    throw new ArgumentException(patronymicLetterConversionResult.Item2, nameof(value));
                }

                searchRecordPatronymicLetter.Add(patronymicLetterConversionResult.Item3);
                return;
            }

            if (key.Equals("income", StringComparison.InvariantCultureIgnoreCase))
            {
                var incomeConversionResult = Converter.ConvertStringToDecimal(value);
                if (!incomeConversionResult.Item1)
                {
                    throw new ArgumentException(incomeConversionResult.Item2, nameof(value));
                }

                searchRecordIncome.Add(incomeConversionResult.Item3);
                return;
            }

            if (key.Equals("height", StringComparison.InvariantCultureIgnoreCase))
            {
                var heightConversionResult = Converter.ConvertStringToShort(value);
                if (!heightConversionResult.Item1)
                {
                    throw new ArgumentException(heightConversionResult.Item2, nameof(value));
                }

                searchRecordHeight.Add(heightConversionResult.Item3);
                return;
            }

            throw new ArgumentException("Wrong property name.", nameof(key));
        }

        private string CreateTable(IEnumerable<FileCabinetRecord> records, IEnumerable<string> fieldsToShow)
        {
            if (!records.Any())
            {
                return "No records with such conditions.";
            }

            if (!fieldsToShow.Any())
            {
                throw new ArgumentException("Wrong parameters.", nameof(fieldsToShow));
            }

            PropertyInfo[] properties = typeof(FileCabinetRecord).GetProperties();
            foreach (string field in fieldsToShow)
            {
                if (!properties.Any(x => x.Name.Equals(field, StringComparison.InvariantCultureIgnoreCase)))
                {
                    throw new ArgumentException("Wrong property name.");
                }
            }

            IEnumerable<PropertyInfo> selectedProperties = properties.Where(x => fieldsToShow.Any(y => y.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase)));
            IEnumerable<int> columnsLengths = this.GetTableColumnLengths(selectedProperties, records);
            StringBuilder table = new StringBuilder();
            StringBuilder separator = new StringBuilder();
            foreach (int length in columnsLengths)
            {
                separator.Append('+');
                for (int i = 0; i < length; i++)
                {
                    separator.Append('-');
                }

                separator.Append('+');
            }

            separator.Append("\n");
            table.Append(separator);
            table.Append(this.GetTableColumnsNames(selectedProperties, columnsLengths));
            table.Append(separator);
            foreach (FileCabinetRecord record in records)
            {
                table.Append(this.GetValuesString(record, columnsLengths, selectedProperties));
                table.Append(separator);
            }

            return table.ToString();
        }

        private IEnumerable<int> GetTableColumnLengths(IEnumerable<PropertyInfo> properties, IEnumerable<FileCabinetRecord> records)
        {
            int maxLength;
            int newMaxLength;
            foreach (var property in properties)
            {
                maxLength = property.Name.Length;
                foreach (var record in records)
                {
                    if ((newMaxLength = property.GetValue(record).ToString().Length) > maxLength)
                    {
                        maxLength = newMaxLength;
                    }
                }

                yield return maxLength;
            }
        }

        private string GetTableColumnsNames(IEnumerable<PropertyInfo> properties, IEnumerable<int> lengths)
        {
            StringBuilder result = new StringBuilder(lengths.Sum() + (properties.Count() * 2));
            int[] lengthsArr = lengths.ToArray();
            int counter = 0;
            int diff;
            foreach (var property in properties)
            {
                if (property.Name.Length == lengthsArr[counter])
                {
                    result.Append('|' + property.Name + '|');
                }
                else
                {
                    result.Append('|' + property.Name);
                    diff = lengthsArr[counter] - property.Name.Length;
                    for (int i = 0; i < diff; i++)
                    {
                        result.Append(' ');
                    }

                    result.Append('|');
                }

                counter++;
            }

            result.Append("\n");
            return result.ToString();
        }

        private string GetValuesString(FileCabinetRecord record, IEnumerable<int> lengths, IEnumerable<PropertyInfo> propertyInfos)
        {
            StringBuilder result = new StringBuilder(lengths.Sum() + (propertyInfos.Count() * 2));
            int[] lengthsArr = lengths.ToArray();
            int counter = 0;
            int diff;
            string valueStr;
            foreach (var property in propertyInfos)
            {
                var value = property.GetValue(record);
                valueStr = value.ToString();
                if (valueStr.Length == lengthsArr[counter])
                {
                    result.Append('|' + valueStr + '|');
                }
                else
                {
                    if (value is string || value is char)
                    {
                        result.Append('|' + valueStr);
                        diff = lengthsArr[counter] - valueStr.Length;
                        for (int i = 0; i < diff; i++)
                        {
                            result.Append(' ');
                        }

                        result.Append('|');
                    }
                    else
                    {
                        result.Append('|');
                        diff = lengthsArr[counter] - valueStr.Length;
                        for (int i = 0; i < diff; i++)
                        {
                            result.Append(' ');
                        }

                        result.Append(valueStr + '|');
                    }
                }

                counter++;
            }

            result.Append("\n");
            return result.ToString();
        }
    }
}
