namespace RolodexAPI.Data;

using Microsoft.EntityFrameworkCore;
using RolodexAPI.Models;

public class AppDbContext : DbContext
{
    public DbSet<Contact> Contacts { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}