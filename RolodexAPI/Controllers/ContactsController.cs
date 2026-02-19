using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RolodexAPI.Data;
using RolodexAPI.Models;

namespace RolodexAPI.Controllers;

[ApiController]
[Route("contacts")]
public class ContactsController : ControllerBase
{
    private readonly AppDbContext _context;
    public ContactsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Contact>>> GetAllContacts() {
        return await _context.Contacts.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Contact>> GetContactById(int id)
    {
        var contact = await _context.Contacts.FindAsync(id);
        if (contact == null)
        {
            return NotFound();
        }
        return contact;
    }

    [HttpPost]
    public async Task<ActionResult<Contact>> CreateContact(Contact contact)
    {
        contact.CreatedAtUtc = DateTime.UtcNow;
        contact.UpdatedAtUtc = DateTime.UtcNow;

        _context.Contacts.Add(contact);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetContactById), new { id = contact.Id }, contact);
    }

    // TODO: Consider adding PATCH endpoint for partial updates (only send changed fields)
    // Current PUT implementation requires sending full Contact object
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateContact(int id, Contact contact)
    {
        if (id != contact.Id)
        {
            return BadRequest();
        }

        contact.UpdatedAtUtc = DateTime.UtcNow;
        _context.Entry(contact).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await ContactExists(id))
            {
                return NotFound();
            }
            throw;
        }
        
        return NoContent();
    }

    private async Task<bool> ContactExists(int id)
    {
        return await _context.Contacts.AnyAsync(c => c.Id == id);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContact(int id)
    {
        var contact = await _context.Contacts.FindAsync(id);
        if (contact == null)
        {
            return NotFound();
        }

        _context.Contacts.Remove(contact);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("upcoming-birthdays")]
    public async Task<ActionResult<IEnumerable<UpcomingBirthdayDto>>> GetUpcomingBirthdays([FromQuery] int days = 30)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var contacts = await _context.Contacts
            .Where(c => c.DateOfBirth != null)
            .ToListAsync();
        
        var upcoming = contacts
            .Select(c => new UpcomingBirthdayDto(
                c.Id,
                c.FirstName,
                c.LastName,
                GetNextBirthday(c.DateOfBirth!.Value, today),
                GetUpcomingAge(c.DateOfBirth!.Value, today)
            ))
            .Where(dto => dto.NextBirthday <= today.AddDays(days))
            .OrderBy(dto => dto.NextBirthday)
            .ToList();

        return Ok(upcoming);
    }

    private DateOnly GetNextBirthday(DateOnly birthDate, DateOnly today)
    {
        var birthday = birthDate;

        // Handle Feb 29 in non-leap years by treating it as Feb 28
        if (birthday.Month == 2 && birthday.Day == 29 && !DateTime.IsLeapYear(today.Year))
        {
            birthday = new DateOnly(today.Year, 2, 28);
        }
        else
        {
            birthday = new DateOnly(today.Year, birthDate.Month, birthDate.Day);
        }

        // If the birthday has already occurred this year, move to the next year
        if (birthday < today)
        {
            birthday = birthday.AddYears(1);
        }

        return birthday;
    }

    private int GetUpcomingAge(DateOnly birthDate, DateOnly today)
    {
        var nextBirthday = GetNextBirthday(birthDate, today);
        return nextBirthday.Year - birthDate.Year;
    }

}