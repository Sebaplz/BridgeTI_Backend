using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private const int MAX_ATTEMPTS = 5;
    private const int LOCK_DURATION_MINUTES = 5;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("register/student")]
    public async Task<IActionResult> RegisterStudent(StudentDto studentDto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == studentDto.Email))
            return Conflict(new { error = "El email ya existe." });

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
            FirstLastName = studentDto.FirstLastName,
            SecondLastName = studentDto.SecondLastName,
            Rut = studentDto.Rut
        };

        _context.Users.Add(user);
        _context.Students.Add(student);

        await _context.SaveChangesAsync();
        return Ok(new {message = "El estudiante se ha registrado con éxito." });
    }

    [HttpPost("register/company")]
    public async Task<IActionResult> RegisterCompany(CompanyDto companyDto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == companyDto.Email))
            return Conflict(new { error = "El email ya existe." });

        var user = new User
        {
            Email = companyDto.Email,
            PasswordHash = HashPassword(companyDto.Password),
            Role = "COMPANY"
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
        return Ok(new {message = "La empresa se ha registrado con éxito." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto, [FromServices] JwtService jwtService)
    {
        // Verificar si la cuenta está bloqueada
        var loginAttempt = await _context.LoginAttempts
            .FirstOrDefaultAsync(la => la.Email == loginDto.Email);

        if (loginAttempt != null && loginAttempt.LockedUntil.HasValue)
        {
            if (DateTime.UtcNow < loginAttempt.LockedUntil)
            {
                var minutesLeft = (loginAttempt.LockedUntil.Value - DateTime.UtcNow).Minutes;
                return StatusCode(429, new
                {
                    error = $"La cuenta está bloqueada. Por favor, inténtalo de nuevo en {minutesLeft} minutos."
                });
            }
            else
            {
                // Si ya pasó el tiempo de bloqueo, reiniciar contador
                loginAttempt.FailedAttempts = 0;
                loginAttempt.LockedUntil = null;
                await _context.SaveChangesAsync();
            }
        }

        // Buscar usuario en la base de datos
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginDto.Email);

        if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            // Incrementar contador de intentos fallidos
            if (loginAttempt == null)
            {
                loginAttempt = new LoginAttempt
                {
                    Email = loginDto.Email,
                    FailedAttempts = 1
                };
                _context.LoginAttempts.Add(loginAttempt);
            }
            else
            {
                loginAttempt.FailedAttempts++;

                // Si alcanza el máximo de intentos, bloquear la cuenta
                if (loginAttempt.FailedAttempts >= MAX_ATTEMPTS)
                {
                    loginAttempt.LockedUntil = DateTime.UtcNow.AddMinutes(LOCK_DURATION_MINUTES);
                    await _context.SaveChangesAsync();
                    return StatusCode(429, new
                    {
                        error = $"Demasiados intentos fallidos. La cuenta está bloqueada por {LOCK_DURATION_MINUTES} minutos."
                    });
                }
            }

            await _context.SaveChangesAsync();
            return Unauthorized(new {error = "Credenciales invalidas!" });
        }

        // Si el login es exitoso, reiniciar el contador de intentos
        if (loginAttempt != null)
        {
            loginAttempt.FailedAttempts = 0;
            loginAttempt.LockedUntil = null;
            await _context.SaveChangesAsync();
        }

        // Generar token JWT y continuar con el proceso normal de login
        var token = jwtService.GenerateToken(user);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(1)
        };

        Response.Cookies.Append("Role", user.Role, cookieOptions);

        return Ok(new { token });
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

