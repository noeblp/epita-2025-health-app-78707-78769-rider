using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
namespace HealthApp.Razor.Data;

public class Admins
{
    [Key]
    public int admin_id { get; set; }
    // Ajoutez d'autres colonnes si nécessaire
}

public class Appointments
{
    [Key]
    public string doctors { get; set; }
    public int patient_id { get; set; }
    public string date { get; set; }
    public string valid { get; set; }
}
public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Admins> Admin { get; set; }
    
    public DbSet<Appointments> Appointment { get; set; }

    
}