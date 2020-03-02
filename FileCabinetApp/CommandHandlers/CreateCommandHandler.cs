using System;
using System.Collections.Generic;
using System.Text;
using FileCabinetApp.Validators;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Command handler to create method.
    /// </summary>
    public class CreateCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">File cabinet service to call.</param>
        public CreateCommandHandler(IFileCabinetService fileCabinetService)
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

            if (commandRequest.Command.Equals("create", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Create(commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
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

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }

        private void Create(string parameters)
        {
            Console.Write("First name: ");
            string firstName = ReadInput<string>(Converter.ConvertStringToString, ValidatorsAgregator.firstNameValidator);
            Console.Write("Last name: ");
            string lastName = ReadInput<string>(Converter.ConvertStringToString, ValidatorsAgregator.lastNameValidator);
            Console.Write("Date of birth: ");
            DateTime dateOfBirth = ReadInput<DateTime>(Converter.ConvertStringToDateTime, ValidatorsAgregator.dateOfBirthValidator);
            Console.Write("Height: ");
            short height = ReadInput<short>(Converter.ConvertStringToShort, ValidatorsAgregator.heightValidator);
            Console.Write("Income: ");
            decimal income = ReadInput<decimal>(Converter.ConvertStringToDecimal, ValidatorsAgregator.incomeValidator);
            Console.Write("Patronymic letter: ");
            char patronymicLetter = ReadInput<char>(Converter.ConvertStringToChar, ValidatorsAgregator.patronymicLetterValidator);
            RecordParametersTransfer transfer = new RecordParametersTransfer(firstName, lastName, dateOfBirth, height, income, patronymicLetter);
            try
            {
                int index = this.Service.CreateRecord(transfer);
                Console.WriteLine($"Record #{index} is created.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
