namespace RolodexAPI.Models;

    public record StaleContactDto
    (
        int Id,
        string FirstName,
        string LastName,
        DateOnly? LastContactedDate
    );