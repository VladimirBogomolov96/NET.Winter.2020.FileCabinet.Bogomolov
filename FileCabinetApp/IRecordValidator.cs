using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Interface to make validation of some parameters.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Validate given parameters.
        /// </summary>
        /// <param name="transfer">Transfer of parameters to validate.</param>
        void ValidateParameters(RecordParametersTransfer transfer);
    }
}
