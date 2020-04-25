using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Command handler to purge method.
    /// </summary>
    public class PurgeCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">File cabinet service to call.</param>
        public PurgeCommandHandler(IFileCabinetService fileCabinetService)
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

            if (commandRequest.Command.Equals("purge", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Purge();
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void Purge()
        {
            try
            {
                int purgedRecords = this.Service.Purge();
                Console.WriteLine("Data file processing is completed: {0} of {1} records were purged.", purgedRecords, purgedRecords + this.Service.GetStat().Item1);
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Defragmentation can be completed only with filesystem service.");
            }
        }
    }
}
