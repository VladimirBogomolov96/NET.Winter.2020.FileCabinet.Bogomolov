using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Command handler to remove method.
    /// </summary>
    public class RemoveCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">File cabinet service to call.</param>
        public RemoveCommandHandler(IFileCabinetService fileCabinetService)
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
                Console.WriteLine("Wrong command line parameter.");
                return;
            }

            if (commandRequest.Command is null)
            {
                Console.WriteLine("Wrong command line parameter.");
                return;
            }

            if (commandRequest.Command.Equals("remove", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Remove(commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Remove(string parameters)
        {
            if (!int.TryParse(parameters, out int id))
            {
                Console.WriteLine("Enter correct ID.");
                return;
            }

            if (this.Service.Remove(id))
            {
                Console.WriteLine("Record #{0} is removed.", id);
            }
            else
            {
                Console.WriteLine("Record #{0} doesn't exist.", id);
            }
        }
    }
}
