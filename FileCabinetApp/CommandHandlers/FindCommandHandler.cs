using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using FileCabinetApp.Printers;

namespace FileCabinetApp.CommandHandlers
{
    public class FindCommandHandler : ServiceCommandHandlerBase
    {
        private Action<IEnumerable<FileCabinetRecord>> printer;

        public FindCommandHandler(IFileCabinetService fileCabinetService, Action<IEnumerable<FileCabinetRecord>> printer)
            : base(fileCabinetService)
        {
            this.printer = printer;
        }

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

            if (commandRequest.Command.Equals("find", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Find(commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Find(string parameters)
        {
            while (true)
            {
                string[] parametersArr = parameters.Split(' ', 2);
                if (parametersArr.Length < 2)
                {
                    Console.WriteLine("Enter property parameter and value to search.");
                    break;
                }

                const int propertyIndex = 0;
                const int searchValueIndex = 1;
                parametersArr[propertyIndex] = parametersArr[propertyIndex].ToUpperInvariant();
                switch (parametersArr[propertyIndex])
                {
                    case "FIRSTNAME":
                        ReadOnlyCollection<FileCabinetRecord> recordsFirstName = this.Service.FindByFirstName(parametersArr[searchValueIndex]);
                        if (recordsFirstName.Count == 0)
                        {
                            Console.WriteLine("Such records don't exist.");
                            break;
                        }

                        this.printer.Invoke(recordsFirstName);
                        break;
                    case "LASTNAME":
                        ReadOnlyCollection<FileCabinetRecord> recordsLastName = this.Service.FindByLastName(parametersArr[searchValueIndex]);
                        if (recordsLastName.Count == 0)
                        {
                            Console.WriteLine("Such records don't exist.");
                            break;
                        }

                        this.printer.Invoke(recordsLastName);
                        break;
                    case "DATEOFBIRTH":
                        DateTime dateOfBirth;
                        try
                        {
                            dateOfBirth = DateTime.ParseExact(parametersArr[searchValueIndex], "MM/dd/yyyy", CultureInfo.InvariantCulture);
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Date of birth must be in the following format: month/day/year");
                            break;
                        }

                        ReadOnlyCollection<FileCabinetRecord> recordsDateOfBirth = this.Service.FindByDateOfbirth(dateOfBirth);
                        if (recordsDateOfBirth.Count == 0)
                        {
                            Console.WriteLine("Such records don't exist.");
                            break;
                        }

                        this.printer.Invoke(recordsDateOfBirth);
                        break;
                    default:
                        Console.WriteLine("Wrong property parameter.");
                        break;
                }

                break;
            }
        }
    }
}
