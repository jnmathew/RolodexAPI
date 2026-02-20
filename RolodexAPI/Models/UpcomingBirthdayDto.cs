namespace RolodexAPI.Models;

public record UpcomingBirthdayDto
(
    int Id,
    string FirstName,
    string LastName,
    DateOnly NextBirthday,
    int UpcomingAge
);
