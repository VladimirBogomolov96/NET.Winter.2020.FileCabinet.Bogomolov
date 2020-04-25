namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Abstract class for service dependent handlers.
    /// </summary>
    public class ServiceCommandHandlerBase : CommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCommandHandlerBase"/> class.
        /// </summary>
        /// <param name="fileCabinetService">File cabinet service to call.</param>
        public ServiceCommandHandlerBase(IFileCabinetService fileCabinetService)
        {
            this.Service = fileCabinetService;
        }

        /// <summary>
        /// Gets file cabinet service.
        /// </summary>
        /// <value>Value of file cabinet service.</value>
        protected IFileCabinetService Service { get; private set; }

        /// <summary>
        /// Handles command line request.
        /// </summary>
        /// <param name="commandRequest">Command line request.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            base.Handle(commandRequest);
        }
    }
}
