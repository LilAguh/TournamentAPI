
using System.ComponentModel.DataAnnotations;
using DataAccess.DAOs.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Models.Exceptions
{
    public class CountryCodeValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var countryCode = value?.ToString();
            if (string.IsNullOrEmpty(countryCode))
            {
                return new ValidationResult("Codigo de pais requerido");
            }

            var countryDao = validationContext.GetService<ICountryDao>();

            if (countryDao == null)
                throw new InvalidOperationException("Servicio de paises no registrado");

            var countryExist = countryDao.CountryExists(countryCode).Result;
            return countryExist ? ValidationResult.Success : new ValidationResult("El Codigo de pais no existe");
        }
    }
}
