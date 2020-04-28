using System;
using System.Globalization;
using System.IO;
using System.Xml;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Command handler to import method.
    /// </summary>
    public class ImportCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">File cabinet service to call.</param>
        public ImportCommandHandler(IFileCabinetService fileCabinetService)
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

            if (commandRequest.Command.Equals("import", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Import(commandRequest.Parameters);
                this.Service.ClearCache();
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
                Console.WriteLine(Configurator.GetConstantString("InvalidInput"));
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
                Console.WriteLine(Configurator.GetConstantString("InvalidFormatType"));
            }
        }
    }
}
