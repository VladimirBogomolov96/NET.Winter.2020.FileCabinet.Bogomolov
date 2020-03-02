using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace FileCabinetApp.CommandHandlers
{
    public class ImportCommandHandler : ServiceCommandHandlerBase
    {
        public ImportCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
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

            if (commandRequest.Command.Equals("import", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Import(commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Import(string parameters)
        {
            string[] parametersArr = parameters.Split(' ', 2);
            if (parametersArr.Length < 2)
            {
                Console.WriteLine("Enter import format and destination file.");
                return;
            }

            const int importTypeIndex = 0;
            const int filePathIndex = 1;

            if (!File.Exists(parametersArr[filePathIndex]))
            {
                Console.WriteLine($"File {parametersArr[filePathIndex]} isn't exist.");
                return;
            }

            FileCabinetServiceSnapshot snapshot = new FileCabinetServiceSnapshot();
            if (parametersArr[importTypeIndex].Equals("csv", StringComparison.OrdinalIgnoreCase))
            {
                using StreamReader fileStream = new StreamReader(parametersArr[filePathIndex]);
                snapshot.LoadFromCsv(fileStream);
                int numberOfImported = this.Service.Restore(snapshot);
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} records were imported from {1}", numberOfImported, parametersArr[filePathIndex]));
            }
            else if (parametersArr[importTypeIndex].Equals("xml", StringComparison.OrdinalIgnoreCase))
            {
                using StreamReader fileStream = new StreamReader(parametersArr[filePathIndex]);
                using XmlReader xmlReader = XmlReader.Create(fileStream);
                snapshot.LoadFromXml(xmlReader);
                int numberOfImported = this.Service.Restore(snapshot);
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} records were imported from {1}", numberOfImported, parametersArr[filePathIndex]));
            }
            else
            {
                Console.WriteLine("Wrong format type.");
            }
        }
    }
}
