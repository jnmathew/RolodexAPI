using Microsoft.AspNetCore.Mvc;
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
}