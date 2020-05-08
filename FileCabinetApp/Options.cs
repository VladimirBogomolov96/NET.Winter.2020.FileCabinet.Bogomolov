namespace FileCabinetApp
{
    /// <summary>
    /// Command line options.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Options"/> class.
        /// </summary>
        public Options()
        {
            this.Rule = Configurator.GetConstantString("DefaultRule");
            this.Storage = Configurator.GetConstantString("DefaultStorage");
            this.Stopwatch = false;
            this.Logger = false;
        }

        /// <summary>
        /// Gets or sets type of validation rules.
        /// </summary>
        /// <value>Type of validation rules.</value>
        public string Rule { get; set; }

        /// <summary>
        /// Gets or sets type of storage.
        /// </summary>
        /// <value>Type of storage.</value>
        public string Storage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether stopwatch is using or not.
        /// </summary>
        /// <value>Whether stopwatch are using or not.</value>
        public bool Stopwatch { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether stopwatch is using or not.
        /// </summary>
        /// <value>Whether stopwatch is using or not.</value>
        public bool Logger { get; set; }
    }
}
