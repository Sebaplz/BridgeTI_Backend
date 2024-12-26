using System.ComponentModel.DataAnnotations;
using BridgeTI;

public class StudentDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100)]
    public string Name { get; set; }

    [Required(ErrorMessage = "El primer apellido es obligatorio")]
    [StringLength(100)]
    public string FirstLastName { get; set; }

    [Required(ErrorMessage = "El segundo apellido es obligatorio")]
    [StringLength(100)]
    public string SecondLastName { get; set; }

    [Required(ErrorMessage = "El RUT es obligatorio")]
    [ChileanRUT]
    public string Rut { get; set; }

    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "Formato de email inválido")]
    public string Email { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [StringLength(20, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 20 caracteres")]
    public string Password { get; set; }
}