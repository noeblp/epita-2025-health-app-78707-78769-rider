using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;
using hospital.Models;
using hospital.Modif_data;
using Microsoft.AspNetCore.Mvc;
using SQLiteConnectionTest;
using Microsoft.AspNetCore.Identity;


namespace hospital.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
        
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        send_mail.SendConfirmationEmail("belperin.n@gmail.com","ravus","ravus");
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
        return View();
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
        
            if (isAuthenticated==1)
            {
                return RedirectToAction("UI_doctor");
            }
            if (isAuthenticated==2)
            {
                return RedirectToAction("UI_patient");
            }

            if (isAuthenticated == 3)
            {
                return RedirectToAction("UI_admin");
            }
            
            else
            {
                ViewBag.ErrorMessage = "Email ou mot de passe incorrect.";
                return View("Login"); 
            }
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