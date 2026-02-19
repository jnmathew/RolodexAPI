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


}