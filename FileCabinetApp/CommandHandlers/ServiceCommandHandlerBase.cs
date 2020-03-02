using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.CommandHandlers
{
    public abstract class ServiceCommandHandlerBase : CommandHandlerBase
    {
        public ServiceCommandHandlerBase(IFileCabinetService fileCabinetService)
        {
            this.Service = fileCabinetService;
        }

        protected IFileCabinetService Service { get; private set; }

        public override void Handle(AppCommandRequest commandRequest)
        {
            base.Handle(commandRequest);
        }
    }
}
