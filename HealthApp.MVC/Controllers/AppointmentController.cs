using System;
using System.Collections.Generic;
using hospital.Models;
using hospital.Modif_data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace hospital.Controllers;

public class AppointmentController:Controller
{
    public IActionResult BookAppo(int? year, int? month, int? week)
    {
        int currentYear = year ?? DateTime.Now.Year;
        int currentMonth = month ?? DateTime.Now.Month;

        DateTime firstDayOfMonth = new DateTime(currentYear, currentMonth, 1);
        int currentWeek = week ?? GetWeekOfMonth(DateTime.Now);

        var firstMonday = firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek + 1);
        if (firstMonday > firstDayOfMonth) firstMonday = firstMonday.AddDays(-7);

        var startOfWeek = firstMonday.AddDays((currentWeek - 1) * 7);
            

            
        List<(string,string,string)> res;
        int? userId = HttpContext.Session.GetInt32("user_id");
        using (var connection = ModifUser.ConnectToDatabase())
        {
            res =GetPatientEvents(connection,userId);
        }


        var events = new List<Calendar>();

        foreach (var c in res)
        {
            (string,string,string) dateStr = c; 
            DateTime date = DateTime.ParseExact(dateStr.Item1, "dd/MM/yyyy", null);
            int j = date.Day;
            int m = date.Month;
            int a = date.Year;
            DateTime jour = DateTime.ParseExact(dateStr.Item2, "HH:mm", null);
            Console.WriteLine("jour=" +jour);
            int h = jour.Hour;
            int ms = jour.Minute;
            Console.WriteLine("hour = " + h+" minute"+ms);
            
            events.Add(new Calendar { Title = dateStr.Item3, Date = new DateTime(a, m, j, h, ms, 0), user_Id = userId});
            Console.WriteLine(j);
        }
            

        ViewBag.CurrentYear = currentYear;
        ViewBag.CurrentMonth = currentMonth;
        ViewBag.CurrentWeek = currentWeek;
        ViewBag.StartOfWeek = startOfWeek;
        ViewBag.Events = events;

        return View();
        
    }
    
    private int GetWeekOfMonth(DateTime date)
    {
        DateTime firstDay = new DateTime(date.Year, date.Month, 1);
        return (date.Day + (int)firstDay.DayOfWeek - 1) / 7 + 1;
    }
    
    static List<(string,string,string)> GetPatientEvents(SqliteConnection connection, int? patientId)
    {
        var query = "SELECT date,hour,name FROM appointment WHERE (patient_id,valid) = (@patient,@letter)";

        using SqliteCommand command = new SqliteCommand(query, connection);
        command.Parameters.AddWithValue("@patient", patientId);
        command.Parameters.AddWithValue("@letter", "A");
        connection.Open();
        
        using SqliteDataReader reader = command.ExecuteReader();
        List<(string,string,string)> dates = new List<(string,string,string)>();
        string date = "";
        string hour = "";
        string name = "";
        while (reader.Read())
        {
            date=reader["date"].ToString();
            hour = reader["hour"].ToString();
            name = reader["name"].ToString();
            dates.Add((date,hour,name));
        }
        connection.Close();
        
        return dates;
    }

    public IActionResult Popup()
    {
        return View();
    }
    
    [HttpPost]
    public ActionResult OpenModal()
    {
        // On définit un ViewBag pour signaler que la fenêtre doit être affichée
        ViewBag.ShowModal = true;
        return View("Popup"); // Renvoie à la vue avec la mini-fenêtre ouverte
    }
    
    
}