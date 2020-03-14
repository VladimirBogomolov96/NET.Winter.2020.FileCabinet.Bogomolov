using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using FileCabinetApp.Printers;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Command handler to find method.
    /// </summary>
    public class FindCommandHandler : ServiceCommandHandlerBase
    {
        private Action<IEnumerable<FileCabinetRecord>> printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">File cabinet service to call.</param>
        /// <param name="printer">Printer to records.</param>
        public FindCommandHandler(IFileCabinetService fileCabinetService, Action<IEnumerable<FileCabinetRecord>> printer)
            : base(fileCabinetService)
        {
            this.printer = printer;
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
                        IEnumerable<FileCabinetRecord> recordsFirstName = this.Service.FindByFirstName(parametersArr[searchValueIndex]);
                        if (!recordsFirstName.Any())
                        {
                            Console.WriteLine("Such records don't exist.");
                            break;
                        }

                        this.printer.Invoke(recordsFirstName);
                        break;
                    case "LASTNAME":
                        IEnumerable<FileCabinetRecord> recordsLastName = this.Service.FindByLastName(parametersArr[searchValueIndex]);
                        if (!recordsLastName.Any())
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

                        IEnumerable<FileCabinetRecord> recordsDateOfBirth = this.Service.FindByDateOfbirth(dateOfBirth);
                        if (!recordsDateOfBirth.Any())
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
