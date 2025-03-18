using System.Collections.Generic;
using hospital.Modif_data;
using hospital.Views.Patient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace hospital.Controllers;

public class PatientController:Controller
{
    public IActionResult Manage()
    {
        return View();
    }
    
    static List<string> GetDoctorList(SqliteConnection connection)
    {
        var query = "SELECT doctor_last_name FROM doctors";

        using SqliteCommand command = new SqliteCommand(query, connection);
        

        connection.Open();
        using SqliteDataReader reader = command.ExecuteReader();

        List<string> last_names = new List<string>();
        while (reader.Read())
        {
            last_names.Add(reader["doctor_last_name"].ToString());
        }

        connection.Close();
        return last_names;
    }



    public IActionResult Search(string doctorName, string specialty)
    {
        List<string> doctorList = new List<string>();

        using (var connection = ModifUser.ConnectToDatabase())
        {
            if (!string.IsNullOrEmpty(doctorName))
            {
                doctorList = GetDoctorList(connection); 
            }
           
        }

        ViewBag.DoctorList = doctorList;
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