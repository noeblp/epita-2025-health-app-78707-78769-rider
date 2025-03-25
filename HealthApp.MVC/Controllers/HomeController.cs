using System;
using System.Diagnostics;
using HealthApp.Razor.Data;
using Microsoft.AspNetCore.Mvc;
using hospital.Models;
using hospital.Modif_data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Appointments = hospital.Models.Appointments;
using HealthApp.Razor.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;

namespace hospital.Controllers;


public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
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
        //send_mail.SendConfirmationEmail("belperin.n@gmail.com","ravus","ravus");
        return View();
    }
    
    
    public IActionResult Register()
    {
        var userI = HttpContext.Session.GetInt32("user_id");
        var userId = HttpContext.Session.GetString("user_first_name");
        var username = HttpContext.Session.GetString("user_last_name");
        var email = HttpContext.Session.GetString("user_email");
        Console.WriteLine(userId + "  "+username+ "  " + email+ "  " + userI);
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    public IActionResult Doctors()
    {
        
        return RedirectToAction("HomeDoctor","Doctor");
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
    
    public IActionResult Logout()
    {
        HttpContext.Session.SetString("IsLoggedIn", "true");
        HttpContext.Session.Clear();
        ViewBag.P = null;
        return RedirectToAction("Index");
    }


    public void push_patient(int id,string email,string role)
    {
        HttpContext.Session.SetInt32("user_id", id);
        HttpContext.Session.SetString("user_email", email);
        HttpContext.Session.SetString("user_role", role);
    }
    
    
    [HttpPost]
    public void SubmitForm(string firstName, string lastName, string email, string password)
    {
        using (var connection = ModifUser.ConnectToDatabase())
        {
                ModifUser.InsertUser(connection, firstName,lastName,email,password);
                int maxId = 0;
                const string max_id = "SELECT MAX(user_id) AS max_id FROM users";
                using (var command = new SqliteCommand(max_id, connection))
                {
                    object result = command.ExecuteScalar();
                    maxId = result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }

                _context.Patient.Add(new Patients
                {
                    patient_email = email, patient_id = maxId, patient_last_name = lastName, patient_name = firstName
                });
                
                
                
                _context.SaveChanges();
        }
        
        
        
    }
    
    
    
    public IActionResult SubmitLogin(string email, string password)
    {
       
        using (var connection = ModifUser.ConnectToDatabase())
        {
            int isAuthenticated = ModifUser.is_user(connection, email, password);
            
            if (isAuthenticated == 1)
            {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                int id = ModifUser.get_id(connection,email);
                string role = ModifUser.get_role(connection,email);
                push_patient(id,email,role);
                return RedirectToAction("HomeDoctor","Doctor");
            }
            if (isAuthenticated == 2)
            {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                int id = ModifUser.get_id(connection,email);
                string role = ModifUser.get_role(connection,email);
                push_patient(id,email,role);
                return RedirectToAction("UI_patient");
            }
            if (isAuthenticated == 3)
            {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                int id = ModifUser.get_id(connection,email);
                push_patient(id,email,"A");
                return RedirectToAction("UI_admin");
            }
            ViewBag.incorrect = true;
            return View("Login"); 
        }
    }

    
    public void SubmitDoctors(string firstName, string lastName, string email, string password)
    {
        using (var connection = ModifUser.ConnectToDatabase())
        {
            //modif_doctors.InsertDoctors(connection, firstName,lastName,email,password);
               
        }
    }
    
    
    
    
    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}