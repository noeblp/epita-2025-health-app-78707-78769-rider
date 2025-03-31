using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using hospital.Models;
using hospital.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;

namespace HealthApp.Razor.Data;













public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Admin> Admin { get; set; }
    
    public DbSet<Appointments> Appointment { get; set; }

    public DbSet<Patient> Patient { get; set; }
    
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<User> Users { get; set; }

    public DbSet<Notifications> Notifications { get; set; }

    
}