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
    
    public (List<string>,List<string>) GetDoctorList(string doctorName, string specialty)
    {
        var query = _context.Doctors.AsQueryable();

        // Apply the conditional filters based on input parameters
        if (!string.IsNullOrEmpty(doctorName))
        {
            query = query.Where(d => d.doctor_last_name.Contains(doctorName)); // Partial match for last name
        }

        if (!string.IsNullOrEmpty(specialty))
        {
            query = query.Where(d => d.doctor_specialty == specialty);
        }

        // Execute the query and select the necessary fields
        var doctors = query
            .Select(d => new
            {
                d.doctor_last_name,
                d.doctor_id
            })
            .ToList();

        // Create the lists from the result
        var lastNames = doctors.Select(d => d.doctor_last_name).ToList();
        var doctorIds = doctors.Select(d => d.doctor_id.ToString()).ToList();

        return (lastNames, doctorIds);
    }
    
    
    [HttpGet]
    public IActionResult Search(string doctorName = null, string specialty = null)
    {
        List<string> doctorList = new List<string>();
        List<string> doctorId = new List<string>();

        // Only query the database if either doctorName or specialty is provided
        if (!string.IsNullOrEmpty(doctorName) || !string.IsNullOrEmpty(specialty))
        {

            (doctorList,doctorId) = GetDoctorList( doctorName, specialty);
            

            ViewBag.DoctorList = doctorList;
            ViewBag.DoctorId = doctorId;
            ViewBag.doctorName = doctorName;
            return View("DoctorSearch");
        }

        // If no search criteria are provided, render the Search view
        return View();
    }



    
    
   
    
}