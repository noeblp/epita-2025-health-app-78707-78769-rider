using Microsoft.AspNetCore.Mvc;

namespace hospital.Controllers;

public class PatientController:Controller
{
    public IActionResult Manage()
    {
        return View();
    }
}