using HealthApp.Razor.Data;
using hospital.Modif_data;
using Microsoft.AspNetCore.Mvc;


namespace hospital.Controllers;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    

    public AdminController( ApplicationDbContext context)
    {
        
        _context = context;
        _context.Database.EnsureCreated();
        
        
    }

    public IActionResult AddDoctor()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Add(string firstname, string lastname, string email,string password, string specialty)
    {
        using (var connection = modif_doctors.ConnectToDatabase())
        {
            modif_doctors.InsertDoctors(connection, firstname, lastname, email, password, specialty);
            return RedirectToAction("AddDoctor");
        }
        // return RedirectToAction("Index", "Home");




    }
}