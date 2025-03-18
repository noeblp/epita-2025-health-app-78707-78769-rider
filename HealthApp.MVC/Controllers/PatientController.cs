using hospital.Modif_data;
using hospital.Views.Patient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hospital.Controllers;

public class PatientController:Controller
{
    public IActionResult Manage()
    {
        return View();
    }


    public void Update(string firstname, string lastname, string email, string password)
    {
        using (var connection = modif_doctors.ConnectToDatabase())
        {
           // modif_patient.UpdateUser(connection, firstname, lastname, email, password);
            
        }
       
    }
}