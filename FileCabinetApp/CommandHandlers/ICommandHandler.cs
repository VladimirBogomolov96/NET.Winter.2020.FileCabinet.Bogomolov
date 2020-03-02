using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Provides methods to command handlers.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// Sets next command handler.
        /// </summary>
        /// <param name="commandHandler">Command handler to set.</param>
        /// <returns>This command handler.</returns>
        ICommandHandler SetNext(ICommandHandler commandHandler);

        /// <summary>
        /// Handles command line request.
        /// </summary>
        /// <param name="commandRequest">Command line request.</param>
        void Handle(AppCommandRequest commandRequest);
    }
}
