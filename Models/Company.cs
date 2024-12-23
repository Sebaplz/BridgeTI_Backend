public class Company
{
    public int CompanyId { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }
    public string ContactName { get; set; }
    public string ContactPhone { get; set; }

    // Relationships
    public User User { get; set; }
}
