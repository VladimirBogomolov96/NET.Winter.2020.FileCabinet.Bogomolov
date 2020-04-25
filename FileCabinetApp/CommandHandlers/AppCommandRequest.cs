namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Request from command line.
    /// </summary>
    public class AppCommandRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppCommandRequest"/> class.
        /// </summary>
        /// <param name="command">Command from command line.</param>
        /// <param name="parameters">Parameters from command line.</param>
        public AppCommandRequest(string command, string parameters)
        {
            this.Command = command;
            this.Parameters = parameters;
        }

        /// <summary>
        /// Gets command value.
        /// </summary>
        /// <value>Command value.</value>
        public string Command { get; }

        /// <summary>
        /// Gets parameters value.
        /// </summary>
        /// <value>Parameters value.</value>
        public string Parameters { get; }
    }
}
