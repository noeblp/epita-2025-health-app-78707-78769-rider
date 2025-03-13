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

    public IActionResult AddDoctor(string first_name, string last_name, string email,string password)
    {
        using (var connection = modif_doctors.ConnectToDatabase())
        {
            modif_doctors.InsertDoctors(connection, first_name,last_name,email,password);
        }
        return View();
    }
}