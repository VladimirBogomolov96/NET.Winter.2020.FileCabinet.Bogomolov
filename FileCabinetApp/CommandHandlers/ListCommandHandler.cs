using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using FileCabinetApp.Printers;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Command handler to list method.
    /// </summary>
    public class ListCommandHandler : ServiceCommandHandlerBase
    {
        private Action<IEnumerable<FileCabinetRecord>> printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">File cabinet service to call.</param>
        /// <param name="printer">Printer to records.</param>
        public ListCommandHandler(IFileCabinetService fileCabinetService, Action<IEnumerable<FileCabinetRecord>> printer)
            : base(fileCabinetService)
        {
            this.printer = printer;
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

            if (commandRequest.Command.Equals("list", StringComparison.InvariantCultureIgnoreCase))
            {
                this.List(commandRequest.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private void List(string parameters)
        {
            ReadOnlyCollection<FileCabinetRecord> records = this.Service.GetRecords();
            if (records.Count is 0)
            {
                Console.WriteLine("There is no stored records.");
            }
            else
            {
                this.printer.Invoke(records);
            }
        }
    }
}
