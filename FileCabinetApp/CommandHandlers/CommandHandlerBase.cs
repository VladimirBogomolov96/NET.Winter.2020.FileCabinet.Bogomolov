using System;
using System.Collections.Generic;
using System.Linq;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Abstract class for command handlers.
    /// </summary>
    public abstract class CommandHandlerBase : ICommandHandler
    {
        private readonly string[] commands = new string[]
        {
            Configurator.GetConstantString("CommandHelp"),
            Configurator.GetConstantString("CommandExit"),
            Configurator.GetConstantString("CommandStat"),
            Configurator.GetConstantString("CommandCreate"),
            Configurator.GetConstantString("CommandExport"),
            Configurator.GetConstantString("CommandImport"),
            Configurator.GetConstantString("CommandPurge"),
            Configurator.GetConstantString("CommandInsert"),
            Configurator.GetConstantString("CommandDelete"),
            Configurator.GetConstantString("CommandUpdate"),
            Configurator.GetConstantString("CommandSelect"),
        };

        private ICommandHandler commandHandler;

        /// <summary>
        /// Handles command line request.
        /// </summary>
        /// <param name="commandRequest">Command line request.</param>
        public virtual void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                return;
            }

            if (this.commandHandler != null)
            {
                this.commandHandler.Handle(commandRequest);
            }
            else
            {
                IEnumerable<string> similarCommands = this.GetSimilarCommands(commandRequest.Command);
                if (!similarCommands.Any())
                {
                    Console.WriteLine($"Command {commandRequest.Command} doesn't exist.");
                }
                else if (similarCommands.Count() == 1)
                {
                    Console.WriteLine($"Command {commandRequest.Command} doesn't exist.");
                    Console.WriteLine($"The most similar command is '{similarCommands.First()}'");
                }
                else
                {
                    Console.WriteLine($"Command {commandRequest.Command} doesn't exist.");
                    Console.WriteLine(Configurator.GetConstantString("SimilarCommands"));
                    foreach (string command in similarCommands)
                    {
                        Console.WriteLine(command);
                    }
                }
            }
        }

        /// <summary>
        /// Sets next command handler.
        /// </summary>
        /// <param name="commandHandler">Command handler to set.</param>
        /// <returns>This command handler.</returns>
        public ICommandHandler SetNext(ICommandHandler commandHandler)
        {
            this.commandHandler = commandHandler;
            return this.commandHandler;
        }

        private static int LevensteinAlgorithm(string input, string command)
        {
            int n = input.Length;
            int m = command.Length;
            int[][] matrix = new int[n + 1][];

            for (int i = 0; i < n + 1; i++)
            {
                matrix[i] = new int[m + 1];
            }

            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            for (int i = 0; i <= n; i++)
            {
                matrix[i][0] = i;
            }

            for (int j = 0; j <= m; j++)
            {
                matrix[0][j] = j;
            }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (command[j - 1] == input[i - 1]) ? 0 : 1;

                    matrix[i][j] = Math.Min(
                        Math.Min(matrix[i - 1][j] + 1, matrix[i][j - 1] + 1),
                        matrix[i - 1][j - 1] + cost);
                }
            }

            return matrix[n][m];
        }

        private IEnumerable<string> GetSimilarCommands(string input)
        {
            foreach (string command in this.commands)
            {
                if (LevensteinAlgorithm(input, command) < 4)
                {
                    yield return command;
                }
            }
        }
    }
}
