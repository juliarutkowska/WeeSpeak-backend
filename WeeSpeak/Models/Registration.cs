namespace WeeSpeak.Models;

public class Registration
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ParentPhoneNumber { get; set; }
    public string Age { get; set; }
    public string Email { get; set; }
    public string SelectedCourse { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
