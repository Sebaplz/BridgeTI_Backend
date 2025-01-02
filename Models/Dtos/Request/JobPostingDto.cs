using System.ComponentModel.DataAnnotations;

public class JobPostingDto
{
    [Required(ErrorMessage = "El t�tulo es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El t�tulo no puede tener m�s de 100 caracteres.")]
    public string Title { get; set; }

    [Required(ErrorMessage = "La descripci�n es obligatoria.")]
    public string Description { get; set; }

    [MaxLength(100, ErrorMessage = "La ubicaci�n no puede tener m�s de 100 caracteres.")]
    public string Location { get; set; }
}