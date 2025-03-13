using System.Data.SQLite;
using System.Diagnostics;
using System.Security.Claims;
using HealthApp.Razor.Data;
using Microsoft.AspNetCore.Mvc;
using hospital.Models;
using hospital.Modif_data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;


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
        
        return View();
    }

    public IActionResult Privacy()
    {
        //send_mail.SendConfirmationEmail("belperin.n@gmail.com","ravus","ravus");
        return View();
    }
    
    
    public IActionResult Register()
    {
        var userId = HttpContext.Session.GetString("user_first_name");
        var username = HttpContext.Session.GetString("user_last_name");
        var email = HttpContext.Session.GetString("user_email");
        Console.WriteLine(userId + "  "+username+ "  " + email);
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    public IActionResult Doctors()
    {
        
        return RedirectToAction("Index","Calendar");
    }

    public IActionResult UI_Patient()
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


    public void push_patient(string email)
    {
        HttpContext.Session.SetString("user_first_name", email);
        HttpContext.Session.SetString("user_last_name", email);
        HttpContext.Session.SetString("user_email", email);
    }
    
    
    [HttpPost]
    public void SubmitForm(string firstName, string lastName, string email, string password)
    {
        using (var connection = ModifUser.ConnectToDatabase())
        {
                ModifUser.InsertUser(connection, firstName,lastName,email,password);
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
                push_patient(email);
                return RedirectToAction("HomeDoctor","Doctor");
            }
            if (isAuthenticated == 2)
            {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                push_patient(email);
                return RedirectToAction("UI_patient");
            }
            if (isAuthenticated == 3)
            {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                push_patient(email);
                return RedirectToAction("UI_admin");
            }
            return View("Index"); 
        }
    }

    
    public void SubmitDoctors(string firstName, string lastName, string email, string password)
    {
        using (var connection = ModifUser.ConnectToDatabase())
        {
            modif_doctors.InsertDoctors(connection, firstName,lastName,email,password);
               
        }
    }
    
    
    
    
    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}