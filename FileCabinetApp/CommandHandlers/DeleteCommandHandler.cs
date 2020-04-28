using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
                Console.WriteLine(Configurator.GetConstantString("InvalidCommand"));
                return;
            }

            if (commandRequest.Command is null)
            {
                Console.WriteLine(Configurator.GetConstantString("InvalidCommand"));
                return;
            }

            if (commandRequest.Command.Equals("delete", StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    IEnumerable<int> result = this.Service.Delete(this.ParseConditions(commandRequest.Parameters));
                    if (!result.Any())
                    {
                        Console.WriteLine(Configurator.GetConstantString("NoRecordsWithValue"));
                    }
                    else
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        foreach (int id in result)
                        {
                            stringBuilder.Append(id).Append(' ');
                        }

                        Console.WriteLine($"Record(s) by index(s) of {stringBuilder}were deleted.");
                        this.Service.ClearCache();
                    }
                }
                catch (ArgumentException)
                {
                    Console.WriteLine(Configurator.GetConstantString("InvalidInput"));
                }
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private IEnumerable<FileCabinetRecord> ParseConditions(string parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters), Configurator.GetConstantString("NullParameters"));
            }

            var temp = parameters.Split("where");
            var arguments = temp[1].Split('=');
            if (arguments.Length != 2)
            {
                throw new ArgumentException(Configurator.GetConstantString("InvalidInput"), nameof(parameters));
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
                    throw new ArgumentException(Configurator.GetConstantString("CantParseParam"), nameof(parameters));
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
                    throw new ArgumentException(Configurator.GetConstantString("CantParseParam"), nameof(parameters));
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
                    throw new ArgumentException(Configurator.GetConstantString("CantParseParam"), nameof(parameters));
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
                    throw new ArgumentException(Configurator.GetConstantString("CantParseParam"), nameof(parameters));
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
                    throw new ArgumentException(Configurator.GetConstantString("CantParseParam"), nameof(parameters));
                }
            }
            else
            {
                throw new ArgumentException(Configurator.GetConstantString("WrongPropertyName"), nameof(parameters));
            }

            return result;
        }
    }
}
