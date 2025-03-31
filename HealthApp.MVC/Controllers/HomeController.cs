using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HealthApp.Razor.Data;
using Microsoft.AspNetCore.Mvc;
using hospital.Models;
using hospital.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Data.Sqlite;


using Microsoft.AspNetCore.Identity;

namespace hospital.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context,
        UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _context.Database.EnsureCreated();
    }

    public IActionResult Index()
    {
        ViewBag.incorrect = false;
        return View();
    }

    
    public IActionResult Privacy()
    {
        HttpContext.Session.Clear();
        return View();
    }

    public IActionResult Register()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    public IActionResult Doctors()
    {
        return RedirectToAction("HomeDoctor", "Doctor");
    }

    public IActionResult UI_patient()
    {
        return View();
    }

    public IActionResult UI_doctor()
    {
        return View();
    }

    public IActionResult UI_admin()
    {
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        HttpContext.Session.SetString("IsLoggedIn", "true");
        HttpContext.Session.Clear();
        await _signInManager.SignOutAsync();
        ViewBag.P = null;
        return RedirectToAction("Index");
    }

    public void push_patient(int id, string email, string role)
    {
        HttpContext.Session.SetInt32("user_id", id);
        HttpContext.Session.SetString("user_email", email);
        HttpContext.Session.SetString("user_role", role);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitForm(string firstName, string lastName, string email, string password)
    {
        var user = new IdentityUser { UserName = email, Email = email};
        var result = await _userManager.CreateAsync(user, password);
        

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "ADMIN");

            _context.Users.Add(new User
            {
                user_email = user.Email,
                user_id = user.Id,
                user_last_name = lastName,
                user_first_name = firstName,
                user_password = password,
                user_role = "A"
            });
            await _context.SaveChangesAsync();
           /* _context.Doctors.Add(new Doctor
            {
                doctor_email = user.Email,
                doctor_id = user.Id,
                doctor_last_name = user.UserName,
                doctor_specialty = "e",
                doctor_first_name = user.UserName
            });
            await _context.SaveChangesAsync();*/
            _context.Patient.Add(new Patient
            {
                patient_email = email,
                patient_id = user.Id,
                patient_last_name = lastName,
                patient_name = firstName
            });

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        
        foreach (var error in result.Errors)
        {
            _logger.LogError($"Error creating user: {error.Description}");
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View("Register");
    }

    [HttpPost]
    public async Task<IActionResult> SubmitLogin(string email, string password)
    {
        
        var user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
            
            if (result.Succeeded)
            {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                HttpContext.Session.SetString("user_email", email);
                
                var roles = await _userManager.GetRolesAsync(user);
                string role = roles.Count > 0 ? roles[0] : "Unknown";
                HttpContext.User.IsInRole("PATIENT");
                HttpContext.Session.SetString("UserRole", role);
                Console.WriteLine("Role: "+role);
                if (role == "DOCTOR")
                {
                   // HttpContext.Session.SetInt32("doctor_id", int.Parse(user.Id));
                    return RedirectToAction("HomeDoctor", "Doctor");
                }
                else if (role == "PATIENT")
                {
                    return RedirectToAction("UI_patient");
                }
                else if (role == "ADMIN")
                {
                    return RedirectToAction("UI_admin");
                }
            }
        }
        
        ViewBag.incorrect = true;
        return View("Login");
    }

    

    
}