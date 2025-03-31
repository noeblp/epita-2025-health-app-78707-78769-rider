using Microsoft.EntityFrameworkCore;

namespace HealthApp.hospital;

public class hospital
{
    public class User
    {
        public int user_id { get; set; }
        public string user_first_name { get; set; }
        public int user_password { get; set; }
        public int user_last_name { get; set; }
        public int user_email { get; set; }
        public int user_role { get; set; }
    }

    public class Patient
    {
        public int patient_id { get; set; }
        public string patient_name { get; set; }
        public string patient_last_name { get; set; }
        public string patient_email { get; set; }
    }

    
    
    
    public class AppDbContext : DbContext
    {
        
        
        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
       
        
        public AppDbContext()
        {

        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {


        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {

                options.Equals("Data Source=hospital.db");

            }

        }
    }
}