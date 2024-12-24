public class LoginAttempt
{
    public int Id { get; set; }
    public string Email { get; set; }
    public int FailedAttempts { get; set; }
    public DateTime? LockedUntil { get; set; }
}