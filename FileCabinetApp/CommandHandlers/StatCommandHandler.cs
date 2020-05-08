using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Command handler to stat method.
    /// </summary>
    public class StatCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">File cabinet service to call.</param>
        public StatCommandHandler(IFileCabinetService fileCabinetService)
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

            if (commandRequest.Command.Equals(Configurator.GetConstantString("CommandStat"), StringComparison.InvariantCultureIgnoreCase))
            {
                this.Stat();
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Stat()
        {
            var recordsCount = this.Service.GetStat();
            Console.WriteLine($"{recordsCount.Item1} record(s). {recordsCount.Item2} removed record(s).");
        }
    }
}
