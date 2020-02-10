using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides method to create validator.
    /// </summary>
    public class FileCabinetCustomService : FileCabinetService
    {
        public FileCabinetCustomService()
            : base(new CustomValidator())
        {
        }
    }
}
