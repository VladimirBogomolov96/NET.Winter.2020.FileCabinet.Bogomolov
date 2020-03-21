using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using FileCabinetApp.Validators;

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
                Console.WriteLine("Wrong command line parameter.");
                return;
            }

            if (commandRequest.Command is null)
            {
                Console.WriteLine("Wrong command line parameter.");
                return;
            }

            if (commandRequest.Command.Equals("insert", StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    Console.WriteLine("Record #{0} was inserted succesfully.", this.Service.Insert(this.ParseRecord(commandRequest.Parameters)));
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
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
                throw new ArgumentNullException(nameof(parameters), "Parameters must be not null.");
            }

            var arguments = parameters.Split("values");
            if (arguments.Length != 2)
            {
                throw new ArgumentException("Wrong insert parameters.", nameof(parameters));
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
                    throw new ArgumentException("Wrong amount of fields.", nameof(parameters));
                }

                if (values.Count != amountOfFields)
                {
                    throw new ArgumentException("Wrong amount of values.", nameof(parameters));
                }

                var idConversionResult = Converter.ConvertStringToInt(values[fields.FindIndex(x => x.Equals("id", StringComparison.InvariantCultureIgnoreCase))]);
                if (!idConversionResult.Item1)
                {
                    throw new ArgumentException(idConversionResult.Item2, nameof(parameters));
                }

                string firstName = values[fields.FindIndex(x => x.Equals("firstname", StringComparison.InvariantCultureIgnoreCase))];
                string lastName = values[fields.FindIndex(x => x.Equals("lastname", StringComparison.InvariantCultureIgnoreCase))];
                var dateOfBirthConversionResult = Converter.ConvertStringToDateTime(values[fields.FindIndex(x => x.Equals("dateOfBirth", StringComparison.InvariantCultureIgnoreCase))]);
                if (!dateOfBirthConversionResult.Item1)
                {
                    throw new ArgumentException(dateOfBirthConversionResult.Item2, nameof(parameters));
                }

                var heightConversionResult = Converter.ConvertStringToShort(values[fields.FindIndex(x => x.Equals("height", StringComparison.InvariantCultureIgnoreCase))]);
                if (!heightConversionResult.Item1)
                {
                    throw new ArgumentException(heightConversionResult.Item2, nameof(parameters));
                }

                var incomeConversionResult = Converter.ConvertStringToDecimal(values[fields.FindIndex(x => x.Equals("income", StringComparison.InvariantCultureIgnoreCase))]);
                if (!incomeConversionResult.Item1)
                {
                    throw new ArgumentException(incomeConversionResult.Item2, nameof(parameters));
                }

                var patronymicLetterConversionResult = Converter.ConvertStringToChar(values[fields.FindIndex(x => x.Equals("patronymicLetter", StringComparison.InvariantCultureIgnoreCase))]);
                if (!patronymicLetterConversionResult.Item1)
                {
                    throw new ArgumentException(patronymicLetterConversionResult.Item2, nameof(parameters));
                }

                return new FileCabinetRecord()
                {
                    Id = idConversionResult.Item3,
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
