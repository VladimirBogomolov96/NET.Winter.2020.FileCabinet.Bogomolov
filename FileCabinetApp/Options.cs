using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace FileCabinetApp
{
    /// <summary>
    /// Command line options.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Gets or sets type of validation rules.
        /// </summary>
        /// <value>Type of validation rules.</value>
        [Option('v', "validation-rules", Default = "default", Required = false)]
        public string Rule { get; set; }

        /// <summary>
        /// Gets or sets type of storage.
        /// </summary>
        /// <value>Type of storage.</value>
        [Option('s', "storage", Default = "memory", Required = false)]
        public string Storage { get; set; }
    }
}
