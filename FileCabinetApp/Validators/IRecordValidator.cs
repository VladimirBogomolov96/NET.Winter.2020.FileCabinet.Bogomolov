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
        /// Validate given record.
        /// </summary>
        /// <param name="record">Record to validate.</param>
        /// <returns>Whether validation was succesful and reason of fail.</returns>
        /// <exception cref="ArgumentNullException">Thrown when record is null.</exception>
        Tuple<bool, string> ValidateParameters(FileCabinetRecord record);
    }
}
