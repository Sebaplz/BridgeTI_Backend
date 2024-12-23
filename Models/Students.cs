public class Student
{
    public int StudentId { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }
    public string University { get; set; }
    public string Career { get; set; }

    // Relationships
    public User User { get; set; }
}
