using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides method to create validator.
    /// </summary>
    public class FileCabinetDefaultService : FileCabinetService
    {
        public FileCabinetDefaultService()
            : base(new DefaultValidator())
        {
        }
    }
}
