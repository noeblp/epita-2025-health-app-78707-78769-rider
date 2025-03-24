using Microsoft.AspNetCore.Mvc;

namespace hospital.Controllers;

public class AppointmentController:Controller
{
    public IActionResult BookAppo()
    {
        return View();
    }
    
    
}