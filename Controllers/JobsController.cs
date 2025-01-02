using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly AppDbContext _context;

    public JobsController(AppDbContext context)
    {
        _context = context;
    }


    [HttpGet("test")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "COMPANY")]
    public IActionResult Test()
    {
        return Ok(new
        {
            message = "Auth success",
            userRole = User.FindFirst("role")?.Value,
            claims = User.Claims.Select(c => new { c.Type, c.Value })
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetJobPostings(int pageNumber = 1, int pageSize = 10)
    {
        var totalRecords = await _context.JobPostings.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

        var jobPostings = await _context.JobPostings
            .Include(jp => jp.Company)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var response = new PaginatedResponse<JobPosting>
        {
            Data = jobPostings,
            TotalRecords = totalRecords,
            PageSize = pageSize,
            PageNumber = pageNumber,
            TotalPages = totalPages
        };

        return Ok(response);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "COMPANY")]
    public async Task<IActionResult> CreateJobPosting(JobPostingDto jobPostingDto)
    {
        if (!ModelState.IsValid) // Validar el DTO
        {
            return BadRequest(ModelState);
        }

        var userEmail = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userEmail == null)
        {
            return BadRequest(new { error = "No se pudo obtener el email del usuario del token JWT." });
        }

        var company = await _context.Companies
            .Include(c => c.User) // Incluir el usuario para acceder al UserId
            .FirstOrDefaultAsync(c => c.User.Email == userEmail);

        if (company == null)
        {
            return NotFound(new { error = "Empresa no encontrada." });
        }

        var jobPosting = new JobPosting
        {
            CompanyId = company.CompanyId, // Usar el CompanyId directamente
            Title = jobPostingDto.Title,
            Description = jobPostingDto.Description,
            Location = jobPostingDto.Location
        };

        _context.JobPostings.Add(jobPosting);
        await _context.SaveChangesAsync();

        return Ok(new { message = "El aviso de trabajo se ha creado con éxito." });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetJobPosting([FromRoute] int id)
    {
        var jobPosting = await _context.JobPostings
            .Include(jp => jp.Company)
            .FirstOrDefaultAsync(jp => jp.InternshipId == id);
        if (jobPosting == null)
        {
            return NotFound(new { error = "Aviso de trabajo no encontrado." });
        }
        return Ok(jobPosting);
    }
}