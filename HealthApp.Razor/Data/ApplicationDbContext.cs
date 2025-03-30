using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HealthApp.Razor.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
<<<<<<< Updated upstream
=======
    
    public DbSet<Admins> Admin { get; set; }
    
    public DbSet<Appointments> Appointment { get; set; }

    public DbSet<Patients> Patient { get; set; }
    
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<User> Users { get; set; }

    public DbSet<Notification> Notifications { get; set; }

    public void UpdateUserRole(int userId, string newRole)
    {
        var user = Users.FirstOrDefault(u => u.user_id == userId);
        if (user != null)
        {
            user.user_role = newRole;
            SaveChanges();
        }
    }
>>>>>>> Stashed changes
}