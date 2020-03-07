using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using FileCabinetApp.Validators;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Command handler to edit method.
    /// </summary>
    public class EditCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">File cabinet service to call.</param>
        public EditCommandHandler(IFileCabinetService fileCabinetService)
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

            if (commandRequest.Command.Equals("edit", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Edit(commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;
                return value;
            }
            while (true);
        }

        private void Edit(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                bool wasFound = false;
                for (int i = 0; i < this.Service.GetRecords().Count; i++)
                {
                    if (this.Service.GetRecords()[i].Id.ToString(CultureInfo.InvariantCulture) == parameters)
                    {
                        wasFound = true;
                        Console.Write("First name: ");
                        string firstName = ReadInput<string>(Converter.ConvertStringToString);
                        Console.Write("Last name: ");
                        string lastName = ReadInput<string>(Converter.ConvertStringToString);
                        Console.Write("Date of birth: ");
                        DateTime dateOfBirth = ReadInput<DateTime>(Converter.ConvertStringToDateTime);
                        Console.Write("Height: ");
                        short height = ReadInput<short>(Converter.ConvertStringToShort);
                        Console.Write("Income: ");
                        decimal income = ReadInput<decimal>(Converter.ConvertStringToDecimal);
                        Console.Write("Patronymic letter: ");
                        char patronymicLetter = ReadInput<char>(Converter.ConvertStringToChar);
                        RecordParametersTransfer transfer = new RecordParametersTransfer(firstName, lastName, dateOfBirth, height, income, patronymicLetter);
                        try
                        {
                            this.Service.EditRecord(Convert.ToInt32(parameters, CultureInfo.InvariantCulture), transfer);
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        break;
                    }
                }

                if (!wasFound)
                {
                    Console.WriteLine($"#{parameters} record is not found.");
                }
            }
            else
            {
                Console.WriteLine("Input record ID to edit.");
            }
        }
    }
}
