using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    public class PurgeCommandHandler : ServiceCommandHandlerBase
    {
        public PurgeCommandHandler(IFileCabinetService fileCabinetService)
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

            if (commandRequest.Command.Equals("purge", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Purge(commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Purge(string parameters)
        {
            if (this.Service is FileCabinetFilesystemService)
            {
                int purgedRecords = this.Service.Purge();
                Console.WriteLine("Data file processing is completed: {0} of {1} records were purged.", purgedRecords, purgedRecords + this.Service.GetStat().Item1);
            }
            else
            {
                Console.WriteLine("Purge command can be used only with Filesystem Service.");
            }
        }
    }
}
