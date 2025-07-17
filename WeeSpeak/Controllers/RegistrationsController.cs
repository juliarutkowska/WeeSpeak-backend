namespace WeeSpeak.Controllers;

using Microsoft.AspNetCore.Mvc;
using WeeSpeak.Models;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class RegistrationsController : ControllerBase
{
    private const string FilePath = "Data/registrations.json";

    [HttpPost]
    public async Task<IActionResult> PostRegistration([FromBody] Registration registration)
    {
        List<Registration> registrations;

        if (System.IO.File.Exists(FilePath))
        {
            var existing = await System.IO.File.ReadAllTextAsync(FilePath);
            registrations = JsonSerializer.Deserialize<List<Registration>>(existing) ?? new List<Registration>();
        }
        else
        {
            registrations = new List<Registration>();
        }

        registrations.Add(registration);
        var json = JsonSerializer.Serialize(registrations, new JsonSerializerOptions { WriteIndented = true });
        Directory.CreateDirectory("Data");
        await System.IO.File.WriteAllTextAsync(FilePath, json);

        return Ok(new { message = "Zapisano zgłoszenie!" });
    }
}
