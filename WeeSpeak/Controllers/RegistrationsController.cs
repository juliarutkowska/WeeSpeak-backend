using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeeSpeak.Data;
using WeeSpeak.Models;

namespace WeeSpeak.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegistrationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RegistrationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // POST: api/registrations
    [HttpPost]
    public async Task<IActionResult> PostRegistration([FromBody] Registration registration)
    {
        _context.Registrations.Add(registration);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Zapisano zgłoszenie w bazie!" });
    }

    // GET: api/registrations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Registration>>> GetRegistrations()
    {
        return await _context.Registrations.ToListAsync();
    }
}