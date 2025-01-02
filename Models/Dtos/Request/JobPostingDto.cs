using System.ComponentModel.DataAnnotations;

public class JobPostingDto
{
    [Required(ErrorMessage = "El título es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El título no puede tener más de 100 caracteres.")]
    public string Title { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    public string Description { get; set; }

    [MaxLength(100, ErrorMessage = "La ubicación no puede tener más de 100 caracteres.")]
    public string Location { get; set; }
}