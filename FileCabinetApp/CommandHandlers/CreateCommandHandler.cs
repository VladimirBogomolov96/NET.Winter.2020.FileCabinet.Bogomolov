using System;

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
                Console.WriteLine(Configurator.GetConstantString("InvalidCommand"));
                return;
            }

            if (commandRequest.Command is null)
            {
                Console.WriteLine(Configurator.GetConstantString("InvalidCommand"));
                return;
            }

            if (commandRequest.Command.Equals("create", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Create();
                this.Service.ClearCache();
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

        private void Create()
        {
            Console.Write(Configurator.GetConstantString("FirstNameColon"));
            string firstName = ReadInput<string>(Converter.ConvertStringToString);
            Console.Write(Configurator.GetConstantString("LastNameColon"));
            string lastName = ReadInput<string>(Converter.ConvertStringToString);
            Console.Write(Configurator.GetConstantString("DateOfBirthColon"));
            DateTime dateOfBirth = ReadInput<DateTime>(Converter.ConvertStringToDateTime);
            Console.Write(Configurator.GetConstantString("HeightColon"));
            short height = ReadInput<short>(Converter.ConvertStringToShort);
            Console.Write(Configurator.GetConstantString("IncomeColon"));
            decimal income = ReadInput<decimal>(Converter.ConvertStringToDecimal);
            Console.Write(Configurator.GetConstantString("PatronymicColon"));
            char patronymicLetter = ReadInput<char>(Converter.ConvertStringToChar);
            RecordParametersTransfer transfer = new RecordParametersTransfer(firstName, lastName, dateOfBirth, height, income, patronymicLetter);
            try
            {
                int index = this.Service.CreateRecord(transfer);
                Console.WriteLine($"Record #{index} is created.");
            }
            catch (ArgumentException)
            {
                Console.WriteLine(Configurator.GetConstantString("InvalidInput"));
            }
        }
    }
}
