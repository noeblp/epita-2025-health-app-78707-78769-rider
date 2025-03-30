using System.Collections.Generic;
using hospital.Modif_data;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace hospital.Controllers;

public class PatientController:Controller
{
    public IActionResult Manage()
    {
        return View();
    }
    
    private static (List<string>,List<string>) GetDoctorList(SqliteConnection connection, string doctorName, string specialty)
    {
        var query = "SELECT doctor_last_name, doctor_specialty,doctor_id FROM doctors WHERE 1=1";
    
        if (!string.IsNullOrEmpty(doctorName))
        {
            query += " AND doctor_last_name LIKE @doctorName";
        }
        if (!string.IsNullOrEmpty(specialty))
        {
            query += " AND doctor_specialty = @specialty";
        }

        using SqliteCommand command = new SqliteCommand(query, connection);
    
        if (!string.IsNullOrEmpty(doctorName))
        {
            command.Parameters.AddWithValue("@doctorName", "%" + doctorName + "%"); // Search for partial match
        }
        if (!string.IsNullOrEmpty(specialty))
        {
            command.Parameters.AddWithValue("@specialty", specialty);
        }

        connection.Open();
        using SqliteDataReader reader = command.ExecuteReader();

        List<string> lastNames = new List<string>();
        List<string> doctorIds = new List<string>();
        while (reader.Read())
        {
            lastNames.Add(reader["doctor_last_name"].ToString());
            doctorIds.Add(reader["doctor_id"].ToString());
        }
        connection.Close();
        return (lastNames,doctorIds);
    }
    
    
    [HttpGet]
    public IActionResult Search(string doctorName = null, string specialty = null)
    {
        List<string> doctorList = new List<string>();
        List<string> doctorId = new List<string>();

        // Only query the database if either doctorName or specialty is provided
        if (!string.IsNullOrEmpty(doctorName) || !string.IsNullOrEmpty(specialty))
        {
            using (var connection = ModifUser.ConnectToDatabase())
            {
                (doctorList,doctorId) = GetDoctorList(connection, doctorName, specialty);
            }

            ViewBag.DoctorList = doctorList;
            ViewBag.DoctorId = doctorId;
            ViewBag.doctorName = doctorName;
            return View("DoctorSearch");
        }

        // If no search criteria are provided, render the Search view
        return View();
    }



    
    
   
    
}