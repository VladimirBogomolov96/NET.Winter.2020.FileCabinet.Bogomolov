using System;
using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Provides extensions for validator builder.
    /// </summary>
    public static class ValidatorBuilderExtension
    {
        /// <summary>
        /// Creates composite validator based on configuration.
        /// </summary>
        /// <param name="validatorBuilder">Builder to create composite validator.</param>
        /// <param name="configuration">Configuration to create validator.</param>
        /// <returns>Composite validator.</returns>
        /// <exception cref="ArgumentNullException">Thrown when validator builder or configuration is null.</exception>
        public static CompositeValidator CreateValidator(this ValidatorBuilder validatorBuilder, IConfiguration configuration)
        {
            if (validatorBuilder is null)
            {
                throw new ArgumentNullException(nameof(validatorBuilder), Configurator.GetConstantString("NullValidatorBuilder"));
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration), Configurator.GetConstantString("NullConfiguration"));
            }

            try
            {
                var firstName = configuration.GetSection("firstName");
                var lastName = configuration.GetSection("lastName");
                var date = configuration.GetSection("dateOfBirth");
                var patronymicLetter = configuration.GetSection("patronymicLetter");
                var income = configuration.GetSection("income");
                var height = configuration.GetSection("height");
                var result = validatorBuilder.ValidateFirstName(Convert.ToInt32(firstName.GetSection("minLength").Value, CultureInfo.InvariantCulture), Convert.ToInt32(firstName.GetSection("maxLength").Value, CultureInfo.InvariantCulture))
                   .ValidateLastName(Convert.ToInt32(lastName.GetSection("minLength").Value, CultureInfo.InvariantCulture), Convert.ToInt32(lastName.GetSection("maxLength").Value, CultureInfo.InvariantCulture))
                   .ValidateDateOfBirth(DateTime.ParseExact(date.GetSection("from").Value, "dd/MM/yyyy", CultureInfo.InvariantCulture), DateTime.ParseExact(date.GetSection("to").Value, "dd/MM/yyyy", CultureInfo.InvariantCulture))
                   .ValidatePatronymic(Convert.ToChar(patronymicLetter.GetSection("from").Value, CultureInfo.InvariantCulture), Convert.ToChar(patronymicLetter.GetSection("to").Value, CultureInfo.InvariantCulture))
                   .ValidateIncome(Convert.ToDecimal(income.GetSection("from").Value, CultureInfo.InvariantCulture), Convert.ToDecimal(income.GetSection("to").Value, CultureInfo.InvariantCulture))
                   .ValidateHeight(Convert.ToInt16(height.GetSection("min").Value, CultureInfo.InvariantCulture), Convert.ToInt16(height.GetSection("max").Value, CultureInfo.InvariantCulture))
                   .Create();

                return result;
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine(Configurator.GetConstantString("InvalidValidationFile"));
                Console.WriteLine(Configurator.GetConstantString("ClosingProgram"));
                Environment.Exit(-1);
            }
            catch (FormatException)
            {
                Console.WriteLine(Configurator.GetConstantString("InvalidValidationFile"));
                Console.WriteLine(Configurator.GetConstantString("ClosingProgram"));
                Environment.Exit(-1);
            }
            catch (OverflowException)
            {
                Console.WriteLine(Configurator.GetConstantString("ValidationOutOfRange"));
                Console.WriteLine(Configurator.GetConstantString("ClosingProgram"));
                Environment.Exit(-1);
            }

            return null;
        }
    }
}
