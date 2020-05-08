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
            new string[] { Configurator.GetConstantString("CommandHelp"), Configurator.GetConstantString("HelpCommandShortHelp"), Configurator.GetConstantString("HelpCommandHelp"), string.Empty },
            new string[] { Configurator.GetConstantString("CommandExit"), Configurator.GetConstantString("ExitCommandShortHelp"), Configurator.GetConstantString("ExitCommandHelp"), string.Empty },
            new string[] { Configurator.GetConstantString("CommandStat"), Configurator.GetConstantString("StatCommandShortHelp"), Configurator.GetConstantString("StatCommandHelp"), string.Empty },
            new string[] { Configurator.GetConstantString("CommandCreate"), Configurator.GetConstantString("CreateCommandShortHelp"), Configurator.GetConstantString("CreateCommandHelp"), string.Empty },
            new string[] { Configurator.GetConstantString("CommandExport"), Configurator.GetConstantString("ExportCommandShortHelp"), Configurator.GetConstantString("ExportCommandHelp"), Configurator.GetConstantString("ExportPatthern") },
            new string[] { Configurator.GetConstantString("CommandImport"), Configurator.GetConstantString("ImportCommandShortHelp"), Configurator.GetConstantString("ImportCommandHelp"), Configurator.GetConstantString("ImportPatthern") },
            new string[] { Configurator.GetConstantString("CommandPurge"), Configurator.GetConstantString("PurgeCommandShortHelp"), Configurator.GetConstantString("PurgeCommandHelp"), string.Empty },
            new string[] { Configurator.GetConstantString("CommandInsert"), Configurator.GetConstantString("InsertCommandShortHelp"), Configurator.GetConstantString("InsertCommandHelp"), Configurator.GetConstantString("InsertPatthern") },
            new string[] { Configurator.GetConstantString("CommandDelete"), Configurator.GetConstantString("DeleteCommandShortHelp"), Configurator.GetConstantString("DeleteCommandHelp"), Configurator.GetConstantString("DeletePatthern") },
            new string[] { Configurator.GetConstantString("CommandUpdate"), Configurator.GetConstantString("UpdateCommandShortHelp"), Configurator.GetConstantString("UpdateCommandHelp"), Configurator.GetConstantString("UpdatePatthern") },
            new string[] { Configurator.GetConstantString("CommandSelect"), Configurator.GetConstantString("SelectCommandShortHelp"), Configurator.GetConstantString("SelectCommandHelp"), Configurator.GetConstantString("SelectPatthern") },
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

            if (commandRequest.Command.Equals(Configurator.GetConstantString("CommandHelp"), StringComparison.InvariantCultureIgnoreCase))
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
