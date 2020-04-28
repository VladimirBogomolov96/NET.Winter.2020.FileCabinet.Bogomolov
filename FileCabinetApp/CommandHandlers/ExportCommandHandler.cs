using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Command handler to export method.
    /// </summary>
    public class ExportCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">File cabinet service to call.</param>
        public ExportCommandHandler(IFileCabinetService fileCabinetService)
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

            if (commandRequest.Command.Equals("export", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Export(commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Export(string parameters)
        {
            string[] parametersArr = parameters.Split(' ', 2);
            if (parametersArr.Length < 2)
            {
                Console.WriteLine(Configurator.GetConstantString("InvalidInput"));
                return;
            }

            const int exportTypeIndex = 0;
            const int filePathIndex = 1;

            if (File.Exists(parametersArr[filePathIndex]))
            {
                Console.WriteLine($"File is exist - rewrite {parametersArr[filePathIndex]}? [Y/n]");
                string answer = Console.ReadLine();
                if (answer.Equals("y", StringComparison.OrdinalIgnoreCase))
                {
                    File.Delete(parametersArr[filePathIndex]);
                }
                else
                {
                    return;
                }
            }

            FileCabinetServiceSnapshot snapshot = this.Service.MakeSnapshot();
            if (parametersArr[exportTypeIndex].Equals("csv", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    using StreamWriter streamWriter = new StreamWriter(parametersArr[filePathIndex]);
                    streamWriter.WriteLine("ID,First Name,Patronymic,Last Name,Date Of Birth,Height,Income");
                    snapshot.SaveToCsv(streamWriter);
                    Console.WriteLine($"All records are exported to file {parametersArr[filePathIndex]}");
                }
                catch (DirectoryNotFoundException)
                {
                    Console.WriteLine(Configurator.GetConstantString("DirectoryNotExist"));
                }
            }
            else if (parametersArr[exportTypeIndex].Equals("xml", StringComparison.OrdinalIgnoreCase))
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "\t",
                };
                try
                {
                    using XmlWriter xmlWriter = XmlWriter.Create(parametersArr[filePathIndex], settings);
                    snapshot.SaveToXml(xmlWriter);
                    Console.WriteLine($"All records are exported to file {parametersArr[filePathIndex]}");
                }
                catch (DirectoryNotFoundException)
                {
                    Console.WriteLine(Configurator.GetConstantString("DirectoryNotExist"));
                }
            }
            else
            {
                Console.WriteLine(Configurator.GetConstantString("InvalidFormatType"));
            }
        }
    }
}
