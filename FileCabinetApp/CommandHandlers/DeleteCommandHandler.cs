using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Command handler to delete method.
    /// </summary>
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">File cabinet service to call.</param>
        public DeleteCommandHandler(IFileCabinetService fileCabinetService)
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

            if (commandRequest.Command.Equals("delete", StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    IEnumerable<int> result = this.Service.Delete(this.ParseConditions(commandRequest.Parameters));
                    if (!result.Any())
                    {
                        Console.WriteLine("There is no records with such value.");
                    }
                    else
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        foreach (int id in result)
                        {
                            stringBuilder.Append(id).Append(' ');
                        }

                        Console.WriteLine("Records by index of {0}were deleted", stringBuilder);
                    }
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception)
                {
                    Console.WriteLine("Wrong input format.");
                }
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private IEnumerable<FileCabinetRecord> ParseConditions(string parameters)
        {
            var temp = parameters.Split("where");
            var arguments = temp[1].Split('=');
            if (arguments.Length != 2)
            {
                throw new ArgumentException("Wrong parameters.", nameof(parameters));
            }

            for (int i = 0; i < arguments.Length; i++)
            {
                arguments[i] = arguments[i].Trim();
            }

            const int fieldIndex = 0;
            const int valueIndex = 1;
            List<FileCabinetRecord> result = new List<FileCabinetRecord>();
            if (arguments[fieldIndex].Equals("id", StringComparison.InvariantCultureIgnoreCase))
            {
                if (int.TryParse(arguments[valueIndex], out int id))
                {
                    foreach (var record in this.Service.GetRecords())
                    {
                        if (record.Id == id)
                        {
                            result.Add(record);
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Can't parse parameters.", nameof(parameters));
                }
            }
            else if (arguments[fieldIndex].Equals("firstname", StringComparison.InvariantCultureIgnoreCase))
            {
                foreach (var record in this.Service.GetRecords())
                {
                    if (record.FirstName == arguments[valueIndex])
                    {
                        result.Add(record);
                    }
                }
            }
            else if (arguments[fieldIndex].Equals("lastname", StringComparison.InvariantCultureIgnoreCase))
            {
                foreach (var record in this.Service.GetRecords())
                {
                    if (record.LastName == arguments[valueIndex])
                    {
                        result.Add(record);
                    }
                }
            }
            else if (arguments[fieldIndex].Equals("dateofbirth", StringComparison.InvariantCultureIgnoreCase))
            {
                if (DateTime.TryParseExact(arguments[valueIndex], "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth))
                {
                    foreach (var record in this.Service.GetRecords())
                    {
                        if (record.DateOfBirth.Year == dateOfBirth.Year && record.DateOfBirth.Month == dateOfBirth.Month && record.DateOfBirth.Day == dateOfBirth.Day)
                        {
                            result.Add(record);
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Can't parse parameters.", nameof(parameters));
                }
            }
            else if (arguments[fieldIndex].Equals("height", StringComparison.InvariantCultureIgnoreCase))
            {
                if (short.TryParse(arguments[valueIndex], out short height))
                {
                    foreach (var record in this.Service.GetRecords())
                    {
                        if (record.Height == height)
                        {
                            result.Add(record);
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Can't parse parameters.", nameof(parameters));
                }
            }
            else if (arguments[fieldIndex].Equals("income", StringComparison.InvariantCultureIgnoreCase))
            {
                if (decimal.TryParse(arguments[valueIndex], out decimal income))
                {
                    foreach (var record in this.Service.GetRecords())
                    {
                        if (record.Income == income)
                        {
                            result.Add(record);
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Can't parse parameters.", nameof(parameters));
                }
            }
            else if (arguments[fieldIndex].Equals("patronymicletter", StringComparison.InvariantCultureIgnoreCase))
            {
                if (char.TryParse(arguments[valueIndex], out char patronymicLetter))
                {
                    foreach (var record in this.Service.GetRecords())
                    {
                        if (record.PatronymicLetter == patronymicLetter)
                        {
                            result.Add(record);
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Can't parse parameters.", nameof(parameters));
                }
            }
            else
            {
                throw new ArgumentException("Wrong property name.", nameof(parameters));
            }

            return result;
        }
    }
}
