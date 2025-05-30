using System.Linq;
using HealthApp.Razor.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
namespace hospital.Controllers;

public class NotificationController:Controller
{
    
    private readonly ApplicationDbContext _context;

    public NotificationController(ApplicationDbContext context)
    {
        _context = context;
    }
    public IActionResult Notif(string id)
    {
        var Notifications = _context.Notifications
            .Where(n => n.patient_id == id)
            .ToList();
        ViewBag.Notifications = Notifications;
        return View();
    }
}