using System;
using System.Collections.Generic;
using System.Text;

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
        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "returns amount of stored records", "The 'stat' command returns amount of stored records." },
            new string[] { "create", "creates new record with entered data", "The 'create' command creates new record with entered data." },
            new string[] { "export", "exports current records into file of given format", "The 'export' command exports current records into file of given format." },
            new string[] { "import", "imports records from given file", "The 'import' command imports records from given file." },
            new string[] { "purge", "defragments file", "The 'purge' command defragments file." },
            new string[] { "insert", "inserts new record with entered data by given id", "The 'insert' command inserts new record with entered data by given id." },
            new string[] { "delete", "deletes records from service", "The 'delete' command deletes records from service." },
            new string[] { "update", "updates records with given values", "The 'update' command updates records with given values." },
            new string[] { "select", "finds records by the given conditions", "The 'select' command finds records by the given conditions." },
        };

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
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }
    }
}
