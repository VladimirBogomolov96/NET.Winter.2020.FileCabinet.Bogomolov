using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Command handler to help method.
    /// </summary>
    public class HelpCommandHandler : CommandHandlerBase
    {
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private const int PatthernIndex = 3;
        private static readonly string[][] HelpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen.", string.Empty },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application.", string.Empty },
            new string[] { "stat", "returns amount of stored records", "The 'stat' command returns amount of stored records.", string.Empty },
            new string[] { "create", "creates new record with entered data", "The 'create' command creates new record with entered data.", string.Empty },
            new string[] { "export", "exports current records into file of given format", "The 'export' command exports current records into file of given format.", Configurator.GetConstantString("ExportPatthern") },
            new string[] { "import", "imports records from given file", "The 'import' command imports records from given file.", Configurator.GetConstantString("ImportPatthern") },
            new string[] { "purge", "defragments file", "The 'purge' command defragments file.", string.Empty },
            new string[] { "insert", "inserts new record with entered data by given id", "The 'insert' command inserts new record with entered data by given id.", Configurator.GetConstantString("InsertPatthern") },
            new string[] { "delete", "deletes records from service", "The 'delete' command deletes records from service.", Configurator.GetConstantString("DeletePatthern") },
            new string[] { "update", "updates records with given values", "The 'update' command updates records with given values.", Configurator.GetConstantString("UpdatePatthern") },
            new string[] { "select", "finds records by the given conditions", "The 'select' command finds records by the given conditions.", Configurator.GetConstantString("SelectPatthern") },
        };

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

            if (commandRequest.Command.Equals("help", StringComparison.InvariantCultureIgnoreCase))
            {
                this.PrintHelp(commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(HelpMessages[index][ExplanationHelpIndex]);
                    Console.WriteLine(HelpMessages[index][PatthernIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine(Configurator.GetConstantString("AvailableCommandsColon"));

                foreach (var helpMessage in HelpMessages)
                {
                    Console.WriteLine($"\t{helpMessage[CommandHelpIndex]}\t- {helpMessage[DescriptionHelpIndex]}");
                }
            }

            Console.WriteLine();
        }
    }
}
