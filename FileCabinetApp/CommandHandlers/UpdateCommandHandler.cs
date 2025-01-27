﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Command handler to update method.
    /// </summary>
    public class UpdateCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">File cabinet service to call.</param>
        public UpdateCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
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

            if (commandRequest.Command.Equals(Configurator.GetConstantString("CommandUpdate"), StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    Console.WriteLine($"{this.Update(commandRequest.Parameters)} records were updated.");
                    this.Service.ClearCache();
                }
                catch (ArgumentException)
                {
                    Console.WriteLine(Configurator.GetConstantString("InvalidInput"));
                    Console.WriteLine(Configurator.GetConstantString("CommandPatthern"));
                    Console.WriteLine(Configurator.GetConstantString("UpdatePatthern"));
                }
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static void SetSearchParameters(List<int> searchRecordId, List<string> searchRecordFirstName, List<string> searchRecordLastName, List<DateTime> searchRecordDateOfBirth, List<char> searchRecordPatronymicLetter, List<decimal> searchRecordIncome, List<short> searchRecordHeight, string key, string value)
        {
            if (key.Equals(Configurator.GetConstantString("ParameterId"), StringComparison.InvariantCultureIgnoreCase))
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

            if (key.Equals(Configurator.GetConstantString("ParameterFirstName"), StringComparison.InvariantCultureIgnoreCase))
            {
                if (searchRecordFirstName.Count > 0)
                {
                    throw new ArgumentException(Configurator.GetConstantString("OneParamToOneProp"), nameof(key));
                }

                searchRecordFirstName.Add(value);
                return;
            }

            if (key.Equals(Configurator.GetConstantString("ParameterLastName"), StringComparison.InvariantCultureIgnoreCase))
            {
                if (searchRecordLastName.Count > 0)
                {
                    throw new ArgumentException(Configurator.GetConstantString("OneParamToOneProp"), nameof(key));
                }

                searchRecordLastName.Add(value);
                return;
            }

            if (key.Equals(Configurator.GetConstantString("ParameterDateOfBirth"), StringComparison.InvariantCultureIgnoreCase))
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

            if (key.Equals(Configurator.GetConstantString("ParameterPatronymicLetter"), StringComparison.InvariantCultureIgnoreCase))
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

            if (key.Equals(Configurator.GetConstantString("ParameterIncome"), StringComparison.InvariantCultureIgnoreCase))
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

            if (key.Equals(Configurator.GetConstantString("ParameterHeight"), StringComparison.InvariantCultureIgnoreCase))
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

        private int Update(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters), Configurator.GetConstantString("NullParameters"));
            }

            if (parameters.Substring(0, 3).Equals(Configurator.GetConstantString("Set"), StringComparison.InvariantCulture))
            {
                parameters = parameters.Remove(0, 3);
            }
            else
            {
                throw new ArgumentException(Configurator.GetConstantString("InvalidInput"), nameof(parameters));
            }

            var arguments = parameters.Split(Configurator.GetConstantString("Where"));
            if (arguments.Length != 2)
            {
                throw new ArgumentException(Configurator.GetConstantString("InvalidInput"), nameof(parameters));
            }

            var fieldsToSet = arguments[0].Split(',');
            IEnumerable<IEnumerable<string>> fieldsAndValuesToSet = fieldsToSet.Select(x => x.Split('=').Select(y => y.Trim()));
            var fieldsToFind = arguments[1].Split(Configurator.GetConstantString("And"));
            IEnumerable<IEnumerable<string>> fieldsAndValuesToFind = fieldsToFind.Select(x => x.Split('=').Select(y => y.Trim()));
            List<int> searchRecordId = new List<int>(1);
            List<string> searchRecordFirstName = new List<string>(1);
            List<string> searchRecordLastName = new List<string>(1);
            List<DateTime> searchRecordDateOfBirth = new List<DateTime>(1);
            List<char> searchRecordPatronymicLetter = new List<char>(1);
            List<decimal> searchRecordIncome = new List<decimal>(1);
            List<short> searchRecordHeight = new List<short>(1);
            foreach (var pair in fieldsAndValuesToFind)
            {
                SetSearchParameters(searchRecordId, searchRecordFirstName, searchRecordLastName, searchRecordDateOfBirth, searchRecordPatronymicLetter, searchRecordIncome, searchRecordHeight, pair.First(), pair.Last());
            }

            IEnumerable<FileCabinetRecord> recordsToUpdate = this.FindRecordsToUpdate(searchRecordId, searchRecordFirstName, searchRecordLastName, searchRecordDateOfBirth, searchRecordPatronymicLetter, searchRecordIncome, searchRecordHeight);
            if (recordsToUpdate is null)
            {
                return 0;
            }
            else
            {
                return this.Service.Update(recordsToUpdate, fieldsAndValuesToSet);
            }
        }

        private IEnumerable<FileCabinetRecord> FindRecordsToUpdate(List<int> searchRecordId, List<string> searchRecordFirstName, List<string> searchRecordLastName, List<DateTime> searchRecordDateOfBirth, List<char> searchRecordPatronymicLetter, List<decimal> searchRecordIncome, List<short> searchRecordHeight)
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
    }
}
