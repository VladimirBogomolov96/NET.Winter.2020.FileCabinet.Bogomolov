using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
                Console.WriteLine(Configurator.GetConstantString("InvalidCommand"));
                return;
            }

            if (commandRequest.Command is null)
            {
                Console.WriteLine(Configurator.GetConstantString("InvalidCommand"));
                return;
            }

            if (commandRequest.Command.Equals("select", StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    string[] memoization = this.GetMemoizationKey(commandRequest.Parameters);
                    string oldResult = this.Service.GetCache(memoization);
                    if (oldResult != null)
                    {
                        Console.WriteLine(oldResult);
                    }
                    else
                    {
                        string result = null;
                        if (string.IsNullOrEmpty(commandRequest.Parameters.Trim()))
                        {
                            result = this.SelectAll();
                        }
                        else
                        {
                            result = this.Select(commandRequest.Parameters);
                        }

                        memoization[8] = result;
                        this.Service.SaveInCache(memoization);
                        Console.WriteLine(result);
                    }
                }
                catch (ArgumentException)
                {
                    Console.WriteLine(Configurator.GetConstantString("InvalidInput"));
                    Console.WriteLine(Configurator.GetConstantString("CommandPatthern"));
                    Console.WriteLine(Configurator.GetConstantString("SelectPatthern"));
                }
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static void SetSearchParametersAnd(List<int> searchRecordId, List<string> searchRecordFirstName, List<string> searchRecordLastName, List<DateTime> searchRecordDateOfBirth, List<char> searchRecordPatronymicLetter, List<decimal> searchRecordIncome, List<short> searchRecordHeight, string key, string value)
        {
            if (key.Equals("id", StringComparison.InvariantCultureIgnoreCase))
            {
                if (searchRecordId.Count > 0)
                {
                    throw new ArgumentException(Configurator.GetConstantString("OneParamToOneProp"), nameof(key));
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
                    throw new ArgumentException(Configurator.GetConstantString("OneParamToOneProp"), nameof(key));
                }

                searchRecordFirstName.Add(value);
                return;
            }

            if (key.Equals("lastname", StringComparison.InvariantCultureIgnoreCase))
            {
                if (searchRecordLastName.Count > 0)
                {
                    throw new ArgumentException(Configurator.GetConstantString("OneParamToOneProp"), nameof(key));
                }

                searchRecordLastName.Add(value);
                return;
            }

            if (key.Equals("dateofbirth", StringComparison.InvariantCultureIgnoreCase))
            {
                if (searchRecordDateOfBirth.Count > 0)
                {
                    throw new ArgumentException(Configurator.GetConstantString("OneParamToOneProp"), nameof(key));
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
                    throw new ArgumentException(Configurator.GetConstantString("OneParamToOneProp"), nameof(key));
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
                    throw new ArgumentException(Configurator.GetConstantString("OneParamToOneProp"), nameof(key));
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
                    throw new ArgumentException(Configurator.GetConstantString("OneParamToOneProp"), nameof(key));
                }

                var heightConversionResult = Converter.ConvertStringToShort(value);
                if (!heightConversionResult.Item1)
                {
                    throw new ArgumentException(heightConversionResult.Item2, nameof(value));
                }

                searchRecordHeight.Add(heightConversionResult.Item3);
                return;
            }

            throw new ArgumentException(Configurator.GetConstantString("WrongPropertyName"), nameof(key));
        }

        private static void SetSearchParametersOr(List<int> searchRecordId, List<string> searchRecordFirstName, List<string> searchRecordLastName, List<DateTime> searchRecordDateOfBirth, List<char> searchRecordPatronymicLetter, List<decimal> searchRecordIncome, List<short> searchRecordHeight, string key, string value)
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

            throw new ArgumentException(Configurator.GetConstantString("WrongPropertyName"), nameof(key));
        }

        private static IEnumerable<int> GetTableColumnLengths(IEnumerable<PropertyInfo> properties, IEnumerable<FileCabinetRecord> records)
        {
            int maxLength;
            int newMaxLength;
            foreach (var property in properties)
            {
                maxLength = property.Name.Length;
                foreach (var record in records)
                {
                    var value = property.GetValue(record);
                    if (value is DateTime temp)
                    {
                        newMaxLength = string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}", temp.Month, temp.Day, temp.Year).Length;
                    }
                    else
                    {
                        newMaxLength = value.ToString().Length;
                    }

                    if (newMaxLength > maxLength)
                    {
                        maxLength = newMaxLength;
                    }
                }

                yield return maxLength;
            }
        }

        private static string GetTableColumnsNames(IEnumerable<PropertyInfo> properties, IEnumerable<int> lengths)
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

        private static string GetValuesString(FileCabinetRecord record, IEnumerable<int> lengths, IEnumerable<PropertyInfo> propertyInfos)
        {
            StringBuilder result = new StringBuilder(lengths.Sum() + (propertyInfos.Count() * 2));
            int[] lengthsArr = lengths.ToArray();
            int counter = 0;
            int diff;
            foreach (var property in propertyInfos)
            {
                var value = property.GetValue(record);
                string valueStr;
                if (value is DateTime temp)
                {
                    valueStr = string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}", temp.Month, temp.Day, temp.Year);
                }
                else
                {
                    valueStr = value.ToString();
                }

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

        private static string CreateTable(IEnumerable<FileCabinetRecord> records, IEnumerable<string> fieldsToShow)
        {
            if (!records.Any())
            {
                return "No records with such conditions.";
            }

            if (!fieldsToShow.Any())
            {
                throw new ArgumentException(Configurator.GetConstantString("WrongParameters"), nameof(fieldsToShow));
            }

            PropertyInfo[] properties = typeof(FileCabinetRecord).GetProperties();
            foreach (string field in fieldsToShow)
            {
                if (!properties.Any(x => x.Name.Equals(field, StringComparison.InvariantCultureIgnoreCase)))
                {
                    throw new ArgumentException(Configurator.GetConstantString("WrongPropertyName"));
                }
            }

            IEnumerable<PropertyInfo> selectedProperties = properties.Where(x => fieldsToShow.Any(y => y.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase)));
            IEnumerable<int> columnsLengths = GetTableColumnLengths(selectedProperties, records);
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
            table.Append(GetTableColumnsNames(selectedProperties, columnsLengths));
            table.Append(separator);
            foreach (FileCabinetRecord record in records)
            {
                table.Append(GetValuesString(record, columnsLengths, selectedProperties));
                table.Append(separator);
            }

            return table.ToString();
        }

        private string Select(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters), Configurator.GetConstantString("NullParameters"));
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
                throw new ArgumentException(Configurator.GetConstantString("WrongParameters"), nameof(parameters));
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
                    throw new ArgumentException(Configurator.GetConstantString("2SearchConditions"), nameof(parameters));
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
                        SetSearchParametersOr(searchRecordId, searchRecordFirstName, searchRecordLastName, searchRecordDateOfBirth, searchRecordPatronymicLetter, searchRecordIncome, searchRecordHeight, pair.First(), pair.Last());
                    }
                }
                else
                {
                    foreach (var pair in fieldsAndValuesToFind)
                    {
                        SetSearchParametersAnd(searchRecordId, searchRecordFirstName, searchRecordLastName, searchRecordDateOfBirth, searchRecordPatronymicLetter, searchRecordIncome, searchRecordHeight, pair.First(), pair.Last());
                    }
                }

                recordsToShow = func.Invoke(searchRecordId, searchRecordFirstName, searchRecordLastName, searchRecordDateOfBirth, searchRecordPatronymicLetter, searchRecordIncome, searchRecordHeight);
            }

            return CreateTable(recordsToShow, fieldsToShow);
        }

        private string SelectAll()
        {
            IEnumerable<FileCabinetRecord> recordsToShow = this.Service.GetRecords();
            List<string> fieldsToShow = new List<string>(7);
            foreach (PropertyInfo info in typeof(FileCabinetRecord).GetProperties())
            {
                fieldsToShow.Add(info.Name);
            }

            return CreateTable(recordsToShow, fieldsToShow);
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

        private string[] GetMemoizationKey(string parameters)
        {
            string[] result = new string[9];
            if (string.IsNullOrEmpty(parameters.Trim()))
            {
                result[0] = string.Empty;
                return result;
            }

            var temp = parameters.Split("where");
            StringBuilder sb = new StringBuilder();
            foreach (string str in temp[0].Split(','))
            {
                sb.Append(str.Trim());
            }

            if (temp.Length == 1)
            {
                result[0] = sb.ToString();
                return result;
            }

            var tempOr = temp[1].Split("or");
            var tempAnd = temp[1].Split("and");
            string type;
            if (tempOr.Length > tempAnd.Length)
            {
                type = "or";
            }
            else
            {
                type = "and";
            }

            sb.Append(type);
            result[0] = sb.ToString();
            foreach (string keyValue in temp[1].Split(type))
            {
                var pair = keyValue.Split('=');
                if (pair.Length != 2)
                {
                    throw new ArgumentException(Configurator.GetConstantString("InvalidInput"), nameof(parameters));
                }

                if (pair[0].Trim().Equals("id", StringComparison.InvariantCultureIgnoreCase))
                {
                    result[1] = pair[1].Trim();
                    continue;
                }
                else if (pair[0].Trim().Equals("firstname", StringComparison.InvariantCultureIgnoreCase))
                {
                    result[2] = pair[1].Trim();
                    continue;
                }
                else if (pair[0].Trim().Equals("lastname", StringComparison.InvariantCultureIgnoreCase))
                {
                    result[3] = pair[1].Trim();
                    continue;
                }
                else if (pair[0].Trim().Equals("dateofbirth", StringComparison.InvariantCultureIgnoreCase))
                {
                    result[4] = pair[1].Trim();
                    continue;
                }
                else if (pair[0].Trim().Equals("patronymicletter", StringComparison.InvariantCultureIgnoreCase))
                {
                    result[5] = pair[1].Trim();
                    continue;
                }
                else if (pair[0].Trim().Equals("income", StringComparison.InvariantCultureIgnoreCase))
                {
                    result[6] = pair[1].Trim();
                    continue;
                }
                else if (pair[0].Trim().Equals("height", StringComparison.InvariantCultureIgnoreCase))
                {
                    result[7] = pair[1].Trim();
                }
            }

            return result;
        }
    }
}
