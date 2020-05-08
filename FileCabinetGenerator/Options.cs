using System;

namespace FileCabinetGenerator
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
            if (int.TryParse(Configurator.GetSetting("DefaultRecordsAmount"), out int amount) && int.TryParse(Configurator.GetSetting("DefaultStartId"), out int id))
            {
                this.RecordsAmount = amount;
                this.StartId = id;
            }
            else
            {
                Console.WriteLine(Configurator.GetConstantString("InvalidAppSettingFile"));
                Console.WriteLine(Configurator.GetConstantString("ClosingProgram"));
                Environment.Exit(-1);
            }
        }

        /// <summary>
        /// Gets or sets type of output file.
        /// </summary>
        /// <value>Type of output file.</value>
        public string OutputType { get; set; } = Configurator.GetConstantString("StandartOutputType");

        /// <summary>
        /// Gets or sets name of output file.
        /// </summary>
        /// <value>Name of output file.</value>
        public string OutputFileName { get; set; } = Configurator.GetConstantString("StandartOutputFileName");

        /// <summary>
        /// Gets or sets amount of records.
        /// </summary>
        /// <value>Amount of records.</value>
        public int RecordsAmount { get; set; }

        /// <summary>
        /// Gets or sets start id.
        /// </summary>
        /// <value>Start id.</value>
        public int StartId { get; set; }
    }
}
