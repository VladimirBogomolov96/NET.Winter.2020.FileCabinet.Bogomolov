using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Provides configuration.
    /// </summary>
    public static class Configurator
    {
        private static readonly IConfigurationRoot AppSettings;
        private static readonly IConfigurationRoot ConstStrings;

        /// <summary>
        /// Initializes static members of the <see cref="Configurator"/> class.
        /// </summary>
#pragma warning disable CA1810 // Initialize reference type static fields inline
        static Configurator()
#pragma warning restore CA1810 // Initialize reference type static fields inline
        {
            try
            {
                AppSettings = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();
            }
            catch (FileNotFoundException)
            {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                Console.WriteLine("Can't find configuration file 'appsettings.json'.\nClosing program...");
                Environment.Exit(-1);
            }
            catch (FormatException)
            {
                Console.WriteLine("Configuration file 'appsettings.json' is invalid.\nClosing program...");
#pragma warning restore CA1303 // Do not pass literals as localized parameters
                Environment.Exit(-1);
            }

            try
            {
                ConstStrings = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile(GetSetting("ConstantStringsFileName"))
                    .Build();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Can't find constant strings file '{GetSetting("ConstantStringsFileName")}'.\nClosing program...");
                Environment.Exit(-1);
            }
            catch (FormatException)
            {
                Console.WriteLine($"Constant strings file '{GetSetting("ConstantStringsFileName")}' is invalid.\nClosing program...");
                Environment.Exit(-1);
            }
        }

        /// <summary>
        /// Provides constant strings values by key.
        /// </summary>
        /// <param name="key">Key to find string.</param>
        /// <returns>String by given key.</returns>
        internal static string GetConstantString(string key)
        {
            string result = ConstStrings.GetSection(key).Value;
            if (result is null)
            {
                Console.WriteLine($"{GetConstantString("MissingString")} {key}");
                Console.WriteLine(GetConstantString("ClosingProgram"));
                Environment.Exit(-1);
            }

            return result;
        }

        /// <summary>
        /// Provides settings values by key.
        /// </summary>
        /// <param name="key">Key to find setting.</param>
        /// <returns>Setting value by given key.</returns>
        internal static string GetSetting(string key)
        {
            string result = AppSettings.GetSection(key).Value;
            if (result is null)
            {
                Console.WriteLine($"Can't find setting by key '{key}' in configuration file 'appsettings.json'.\nClosing program...");
                Environment.Exit(-1);
            }

            return result;
        }
    }
}
