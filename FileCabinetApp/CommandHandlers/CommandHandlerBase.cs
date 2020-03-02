using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Abstract class for command handlers.
    /// </summary>
    public abstract class CommandHandlerBase : ICommandHandler
    {
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
                Console.WriteLine($"Command {commandRequest.Command} doesn't exit.");
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
    }
}
