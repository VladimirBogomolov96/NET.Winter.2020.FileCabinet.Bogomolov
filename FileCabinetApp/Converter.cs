using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Converter to values.
    /// </summary>
    public static class Converter
    {
        /// <summary>
        /// Converts string to date.
        /// </summary>
        /// <param name="input">String to convert.</param>
        /// <returns>Whether convertion was succesfull, reason of fail and result.</returns>
        public static Tuple<bool, string, DateTime> ConvertStringToDateTime(string input)
        {
            bool isConverted = DateTime.TryParseExact(input, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth);
            if (isConverted)
            {
                return new Tuple<bool, string, DateTime>(isConverted, string.Empty, dateOfBirth);
            }
            else
            {
                return new Tuple<bool, string, DateTime>(isConverted, "DateTime must be in format MM/dd/yyyy", DateTime.MinValue);
            }
        }

        /// <summary>
        /// Converts string to short.
        /// </summary>
        /// <param name="input">String to convert.</param>
        /// <returns>Whether convertion was succesfull, reason of fail and result.</returns>
        public static Tuple<bool, string, short> ConvertStringToShort(string input)
        {
            bool isConverted = short.TryParse(input, out short height);
            if (isConverted)
            {
                return new Tuple<bool, string, short>(isConverted, string.Empty, height);
            }
            else
            {
                return new Tuple<bool, string, short>(isConverted, "Short must be from -32768 to 32767", short.MinValue);
            }
        }

        /// <summary>
        /// Converts string to decimal.
        /// </summary>
        /// <param name="input">String to convert.</param>
        /// <returns>Whether convertion was succesfull, reason of fail and result.</returns>
        public static Tuple<bool, string, decimal> ConvertStringToDecimal(string input)
        {
            bool isConverted = decimal.TryParse(input, out decimal income);
            if (isConverted)
            {
                return new Tuple<bool, string, decimal>(isConverted, string.Empty, income);
            }
            else
            {
                return new Tuple<bool, string, decimal>(isConverted, "Decimal must be from (+/-)1.0*10^-28 to (+/-)7.9228*10^28", decimal.MinValue);
            }
        }

        /// <summary>
        /// Converts string to char.
        /// </summary>
        /// <param name="input">String to convert.</param>
        /// <returns>Whether convertion was succesfull, reason of fail and result.</returns>
        public static Tuple<bool, string, char> ConvertStringToChar(string input)
        {
            bool isConverted = char.TryParse(input, out char patronymicLetter);
            if (isConverted)
            {
                return new Tuple<bool, string, char>(isConverted, string.Empty, patronymicLetter);
            }
            else
            {
                return new Tuple<bool, string, char>(isConverted, "Char must be a single Unicode symbol", char.MinValue);
            }
        }

        /// <summary>
        /// Converts string to string.
        /// </summary>
        /// <param name="input">String to convert.</param>
        /// <returns>Whether convertion was succesfull, reason of fail and result.</returns>
        public static Tuple<bool, string, string> ConvertStringToString(string input)
        {
            return new Tuple<bool, string, string>(true, string.Empty, input);
        }
    }
}
