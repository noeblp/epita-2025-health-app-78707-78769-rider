using System;
using System.Linq;
using HealthApp.Razor.Data;
using Microsoft.AspNetCore.Mvc;

namespace hospital.Modif_data;

public class VotreEntitéController : Controller
{
    private readonly ApplicationDbContext _context;

    public VotreEntitéController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Login()
    {
        var entités = _context.Users.ToList();  
        
        Console.WriteLine("hihi");
        return View(entités);  
    }
}