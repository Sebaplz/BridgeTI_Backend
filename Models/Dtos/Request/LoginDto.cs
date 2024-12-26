using System.ComponentModel.DataAnnotations;

public class LoginDto
{
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "Formato de email inv�lido")]
    public string Email { get; set; }

    [Required(ErrorMessage = "La contrase�a es obligatoria")]
    [StringLength(20, MinimumLength = 8, ErrorMessage = "La contrase�a debe tener entre 8 y 20 caracteres")]
    public string Password { get; set; }
}