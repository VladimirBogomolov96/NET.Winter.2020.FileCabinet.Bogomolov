using System;
using System.Linq;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Command handler to insert method.
    /// </summary>
    public class InsertCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">File cabinet service to call.</param>
        public InsertCommandHandler(IFileCabinetService fileCabinetService)
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

            if (commandRequest.Command.Equals(Configurator.GetConstantString("CommandInsert"), StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    Console.WriteLine($"Record #{this.Service.Insert(this.ParseRecord(commandRequest.Parameters))} was inserted succesfully.");
                    this.Service.ClearCache();
                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine(Configurator.GetConstantString("RecordIdExist"));
                }
                catch (ArgumentException)
                {
                    Console.WriteLine(Configurator.GetConstantString("InvalidInput"));
                    Console.WriteLine(Configurator.GetConstantString("CommandPatthern"));
                    Console.WriteLine(Configurator.GetConstantString("InsertPatthern"));
                }
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private FileCabinetRecord ParseRecord(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters), Configurator.GetConstantString("NullParameters"));
            }

            var arguments = parameters.Split(Configurator.GetConstantString("Values"));
            if (arguments.Length != 2)
            {
                throw new ArgumentException(Configurator.GetConstantString("InvalidInput"), nameof(parameters));
            }
            else
            {
                int amountOfFields = typeof(FileCabinetRecord).GetProperties().Length;
                var fields = arguments[0].Split(',', ')', '(').ToList();
                var values = arguments[1].Split(',', ')', '(').ToList();
                fields.RemoveAll(x => x.Trim().Length == 0);
                values.RemoveAll(x => x.Trim().Length == 0);
                for (int i = 0; i < fields.Count; i++)
                {
                    fields[i] = fields[i].Trim();
                }

                for (int i = 0; i < values.Count; i++)
                {
                    values[i] = values[i].Trim();
                }

                if (fields.Count != amountOfFields)
                {
                    throw new ArgumentException(Configurator.GetConstantString("WrongFieldsAmount"), nameof(parameters));
                }

                if (values.Count != amountOfFields)
                {
                    throw new ArgumentException(Configurator.GetConstantString("WrongValuesAmount"), nameof(parameters));
                }

                var conversionResultOfId = Converter.ConvertStringToInt(values[fields.FindIndex(x => x.Equals(Configurator.GetConstantString("ParameterId"), StringComparison.InvariantCultureIgnoreCase))]);
                if (!conversionResultOfId.Item1)
                {
                    throw new ArgumentException(conversionResultOfId.Item2, nameof(parameters));
                }

                string firstName = values[fields.FindIndex(x => x.Equals(Configurator.GetConstantString("ParameterFirstName"), StringComparison.InvariantCultureIgnoreCase))];
                string lastName = values[fields.FindIndex(x => x.Equals(Configurator.GetConstantString("ParameterLastName"), StringComparison.InvariantCultureIgnoreCase))];
                var dateOfBirthConversionResult = Converter.ConvertStringToDateTime(values[fields.FindIndex(x => x.Equals(Configurator.GetConstantString("ParameterDateOfBirth"), StringComparison.InvariantCultureIgnoreCase))]);
                if (!dateOfBirthConversionResult.Item1)
                {
                    throw new ArgumentException(dateOfBirthConversionResult.Item2, nameof(parameters));
                }

                var heightConversionResult = Converter.ConvertStringToShort(values[fields.FindIndex(x => x.Equals(Configurator.GetConstantString("ParameterHeight"), StringComparison.InvariantCultureIgnoreCase))]);
                if (!heightConversionResult.Item1)
                {
                    throw new ArgumentException(heightConversionResult.Item2, nameof(parameters));
                }

                var incomeConversionResult = Converter.ConvertStringToDecimal(values[fields.FindIndex(x => x.Equals(Configurator.GetConstantString("ParameterIncome"), StringComparison.InvariantCultureIgnoreCase))]);
                if (!incomeConversionResult.Item1)
                {
                    throw new ArgumentException(incomeConversionResult.Item2, nameof(parameters));
                }

                var patronymicLetterConversionResult = Converter.ConvertStringToChar(values[fields.FindIndex(x => x.Equals(Configurator.GetConstantString("ParameterPatronymicLetter"), StringComparison.InvariantCultureIgnoreCase))]);
                if (!patronymicLetterConversionResult.Item1)
                {
                    throw new ArgumentException(patronymicLetterConversionResult.Item2, nameof(parameters));
                }

                return new FileCabinetRecord()
                {
                    Id = conversionResultOfId.Item3,
                    FirstName = firstName,
                    LastName = lastName,
                    DateOfBirth = dateOfBirthConversionResult.Item3,
                    Height = heightConversionResult.Item3,
                    Income = incomeConversionResult.Item3,
                    PatronymicLetter = patronymicLetterConversionResult.Item3,
                };
            }
        }
    }
}
