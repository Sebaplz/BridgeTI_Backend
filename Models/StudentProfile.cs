public class StudentProfile
{
    public int StudentProfileId { get; set; }
    public int StudentId { get; set; }

    // Datos adicionales para postulación
    public string University { get; set; }
    public string Career { get; set; }
    public int CurrentYear { get; set; }
    public string InstitutionalEmail { get; set; }
    public string Address { get; set; }
    public string Comuna { get; set; }
    public string Region { get; set; }
    public string Country { get; set; }
    public string PhoneNumber { get; set; }
    public string ProfilePhotoUrl { get; set; }
    public string HowDidYouFindUs { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Relationship
    public Student Student { get; set; }
}