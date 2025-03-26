﻿using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
namespace HealthApp.Razor.Data;

public class Admins
{
    [Key]
    public int admin_id { get; set; }
}

public class Patients
{
    [Key]
    public int patient_id { get; set; }
    public string patient_name { get; set; }
    public string patient_last_name { get; set; }
    public string patient_email { get; set; }
}

public class Doctor
{
    [Key]
    public int doctor_id { get; set; }
    public string doctor_first_name { get; set; }
    public string doctor_last_name { get; set; }
    public string doctor_email { get; set; }
    public string doctor_specialty { get; set; }
}
public class Appointments
{
    [Key]
    public int? doctor_id { get; set; }
    public int? patient_id { get; set; }
    public string date { get; set; }
    public string valid { get; set; }
    public string hour { get; set; }
    
    public string name { get; set; }
    
    public int appo_id { get; set; }
    
}
public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Admins> Admin { get; set; }
    
    public DbSet<Appointments> Appointment { get; set; }

    public DbSet<Patients> Patient { get; set; }
    
    public DbSet<Doctor> Doctors { get; set; }
    
}