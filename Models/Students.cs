public class Student
{
    public int StudentId { get; set; }
    public int UserId { get; set; }

    // Datos de registro obligatorios
    public string Name { get; set; }
    public string FirstLastName { get; set; }
    public string SecondLastName { get; set; }
    public string Rut { get; set; }

    // Relationships
    public User User { get; set; }
    public StudentProfile Profile { get; set; }
}