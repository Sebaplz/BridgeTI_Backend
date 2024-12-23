using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("register/student")]
    public async Task<IActionResult> RegisterStudent(StudentDto studentDto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == studentDto.Email))
            return BadRequest("Email already exists.");

        var user = new User
        {
            Email = studentDto.Email,
            PasswordHash = HashPassword(studentDto.Password),
            Role = "STUDENT"
        };

        var student = new Student
        {
            User = user,
            Name = studentDto.Name,
            University = studentDto.University,
            Career = studentDto.Career
        };

        _context.Users.Add(user);
        _context.Students.Add(student);

        await _context.SaveChangesAsync();
        return Ok("Student registered successfully.");
    }

    [HttpPost("register/company")]
    public async Task<IActionResult> RegisterCompany(CompanyDto companyDto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == companyDto.Email))
            return BadRequest("Email already exists.");

        var user = new User
        {
            Email = companyDto.Email,
            PasswordHash = HashPassword(companyDto.Password),
            Role = "COMPANY_ADMIN"
        };

        var company = new Company
        {
            User = user,
            Name = companyDto.CompanyName,
            ContactName = companyDto.ContactName,
            ContactPhone = companyDto.ContactPhone
        };

        _context.Users.Add(user);
        _context.Companies.Add(company);

        await _context.SaveChangesAsync();
        return Ok("Company registered successfully.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginDto.Email);
        if (user == null || user.PasswordHash != HashPassword(loginDto.Password))
            return Unauthorized("Invalid credentials.");

        return Ok("Login successful.");
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
    }

    private bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}

