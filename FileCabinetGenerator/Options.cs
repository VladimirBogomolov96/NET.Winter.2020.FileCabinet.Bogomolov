namespace FileCabinetGenerator
{
    /// <summary>
    /// Command line options.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Gets or sets type of output file.
        /// </summary>
        /// <value>Type of output file.</value>
        public string OutputType { get; set; } = "csv";

        /// <summary>
        /// Gets or sets name of output file.
        /// </summary>
        /// <value>Name of output file.</value>
        public string OutputFileName { get; set; } = "NewFile";

        /// <summary>
        /// Gets or sets amount of records.
        /// </summary>
        /// <value>Amount of records.</value>
        public int RecordsAmount { get; set; } = 10;

        /// <summary>
        /// Gets or sets start id.
        /// </summary>
        /// <value>Start id.</value>
        public int StartId { get; set; } = 1;
    }
}
