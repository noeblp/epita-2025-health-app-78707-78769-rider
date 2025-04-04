using System.Collections.Generic;
using System.Linq;
using HealthApp.Razor.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace hospital.Controllers;

public class PatientController:Controller
{
    
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    

    public PatientController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _context.Database.EnsureCreated();
        
        
    }
    public IActionResult Manage()
    {
        return View();
    }
    
    public (List<string>,List<string>,List<string>) GetDoctorList(string doctorName, string specialty)
    {
        var query = _context.Doctors.AsQueryable();

        
        if (!string.IsNullOrEmpty(doctorName))
        {
            query = query.Where(d => d.doctor_last_name.Contains(doctorName)); 
        }

        if (!string.IsNullOrEmpty(specialty))
        {
            query = query.Where(d => d.doctor_specialty == specialty);
        }

        
        var doctors = query
            .Select(d => new
            {
                d.doctor_last_name,
                d.doctor_id,
                d.doctor_specialty
            })
            .ToList();

        
        var lastNames = doctors.Select(d => d.doctor_last_name).ToList();
        var doctorIds = doctors.Select(d => d.doctor_id.ToString()).ToList();
        var doctorSpe=doctors.Select(d=>d.doctor_specialty).ToList();

        return (lastNames, doctorIds,doctorSpe);
    }
    
    
    [HttpGet]
    public IActionResult Search(string doctorName = null, string specialty = null)
    {
        List<string> doctorList = new List<string>();
        List<string> doctorId = new List<string>();
        List<string> doctorSpe = new List<string>();

        if (!string.IsNullOrEmpty(doctorName) || !string.IsNullOrEmpty(specialty))
        {

            (doctorList,doctorId,doctorSpe) = GetDoctorList( doctorName, specialty);
            

            ViewBag.DoctorList = doctorList;
            ViewBag.DoctorId = doctorId;
            ViewBag.DoctorSpe = doctorSpe;
            ViewBag.doctorName = doctorName;
            return View("DoctorSearch");
        }

        return View();
    }



    
    
   
    
}