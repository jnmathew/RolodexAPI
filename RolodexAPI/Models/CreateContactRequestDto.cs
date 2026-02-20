using System.ComponentModel.DataAnnotations;

namespace RolodexAPI.Models;

public class CreateContactRequest
{
    [Required]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public ContactType? Type { get; set; }
    public string? Address { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public DateOnly? LastContactedDate { get; set; }
    public string? Notes { get; set; }
}
