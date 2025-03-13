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

public class DoctorController : Controller
{
   
    private readonly ApplicationDbContext _context;
    

    public DoctorController( ApplicationDbContext context)
    {
        
        _context = context;
        _context.Database.EnsureCreated();
        
        
    }

    public IActionResult HomeDoctor()
    {
        return View();
    }

    
    
    
    
    
    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}