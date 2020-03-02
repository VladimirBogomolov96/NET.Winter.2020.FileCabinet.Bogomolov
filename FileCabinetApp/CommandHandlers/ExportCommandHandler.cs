﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace FileCabinetApp.CommandHandlers
{
    public class ExportCommandHandler : ServiceCommandHandlerBase
    {
        public ExportCommandHandler(IFileCabinetService fileCabinetService)
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
                Console.WriteLine("Enter export format and destination file.");
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
                catch (DirectoryNotFoundException ex)
                {
                    Console.WriteLine(ex.Message);
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
                catch (DirectoryNotFoundException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Wrong format type.");
            }
        }
    }
}