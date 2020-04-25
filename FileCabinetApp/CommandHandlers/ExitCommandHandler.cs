using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Command handler to exit method.
    /// </summary>
    public class ExitCommandHandler : CommandHandlerBase
    {
        private readonly Action<bool> action;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitCommandHandler"/> class.
        /// </summary>
        /// <param name="action">Delegate to invoke.</param>
        public ExitCommandHandler(Action<bool> action)
        {
            this.action = action;
        }

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

            if (commandRequest.Command.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Exit();
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Exit()
        {
            Console.WriteLine("Exiting an application...");
            this.action(false);
        }
    }
}
