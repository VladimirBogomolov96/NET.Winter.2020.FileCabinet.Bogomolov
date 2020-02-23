using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

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
        [Option('t', "output-type", Default = "csv", Required = true)]
        public string OutputType { get; set; }

        /// <summary>
        /// Gets or sets name of output file.
        /// </summary>
        /// <value>Name of output file.</value>
        [Option('o', "output", Default = "NewFile", Required = true)]
        public string OutputFileName { get; set; }

        /// <summary>
        /// Gets or sets amount of records.
        /// </summary>
        /// <value>Amount of records.</value>
        [Option('a', "records-amount", Default = 10, Required = false)]
        public int RecordsAmount { get; set; }

        /// <summary>
        /// Gets or sets start id.
        /// </summary>
        /// <value>Start id.</value>
        [Option('i', "start-id	", Default = 1, Required = false)]
        public int StartId { get; set; }
    }
}
