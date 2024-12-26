using System;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

public static class ValidateRUT
{
    public static ValidationResult Validate(string rut)
    {
        if (string.IsNullOrWhiteSpace(rut))
        {
            return new ValidationResult("El RUT es requerido.");
        }

        // Limpiar el RUT (remover puntos y guiones)
        rut = rut.Replace(".", "").Replace("-", "").ToUpper();

        // Validar formato básico: Dígitos y un dígito verificador (DV)
        if (!Regex.IsMatch(rut, @"^\d{7,8}[0-9K]$"))
        {
            return new ValidationResult("El formato del RUT no es válido.");
        }

        // Separar número y DV
        var numberPart = rut.Substring(0, rut.Length - 1);
        var dvProvided = rut[^1].ToString();

        // Calcular DV esperado
        var dvCalculated = CalculateDV(numberPart);

        // Validar DV
        if (dvCalculated != dvProvided)
        {
            return new ValidationResult("El RUT no es válido.");
        }

        return ValidationResult.Success;
    }

    private static string CalculateDV(string numberPart)
    {
        var sum = 0;
        var factor = 2;

        for (int i = numberPart.Length - 1; i >= 0; i--)
        {
            sum += (numberPart[i] - '0') * factor;
            factor = factor == 7 ? 2 : factor + 1;
        }

        var remainder = 11 - (sum % 11);

        return remainder switch
        {
            11 => "0",
            10 => "K",
            _ => remainder.ToString()
        };
    }
}

public class ChileanRUTAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return new ValidationResult("El RUT es requerido.");
        }

        var rut = value.ToString();
        return ValidateRUT.Validate(rut);
    }
}
