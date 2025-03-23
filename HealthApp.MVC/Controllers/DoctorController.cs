using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using HealthApp.Razor.Data;
using Microsoft.AspNetCore.Mvc;
using hospital.Models;
using hospital.Modif_data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Calendar = hospital.Models.Calendar;


namespace hospital.Controllers;

public class DoctorController : Controller
{
   
    private readonly ApplicationDbContext _context;
    

    public DoctorController( ApplicationDbContext context)
    {
        
        _context = context;
        _context.Database.EnsureCreated();
        
        
    }
    
    

    public IActionResult HomeDoctor(int? year, int? month, int? week, int? day)
    {
        int currentYear = year ?? DateTime.Now.Year;
        int currentMonth = month ?? DateTime.Now.Month;
        int currentDay = day ?? DateTime.Now.Day;

        DateTime firstDayOfMonth = new DateTime(currentYear, currentMonth, 1);
        int currentWeek = week ?? GetWeekOfMonth2(DateTime.Now);

        var firstMonday = firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek + 1);
        if (firstMonday > firstDayOfMonth) firstMonday = firstMonday.AddDays(-7);

        var startOfWeek = firstMonday.AddDays((currentWeek - 1) * 7);
            

            
        List<string> res;
        List<string> res2;
        int? userId = HttpContext.Session.GetInt32("user_id");
        using (var connection = ModifUser.ConnectToDatabase())
        {
            res =GetPatientEvents(connection,userId);
            res2=GetPatientEventsNew(connection,userId);
        }


        var events = new List<Calendar>();
        var newEvents = new List<Calendar>();

        foreach (var c in res)
        {
            string dateStr = c; 
            DateTime date = DateTime.ParseExact(dateStr, "dd/MM/yyyy", null);

            int j = date.Day;
            int m = date.Month;
            int a = date.Year;
            events.Add(new Calendar { Title = "Réunion", Date = new DateTime(a, m, j, 11, 30, 0), user_Id = userId});
            Console.WriteLine(j);
        }
        foreach (var c in res2)
        {
            string dateStr = c; 
            DateTime date = DateTime.ParseExact(dateStr, "dd/MM/yyyy", null);

            int j = date.Day;
            int m = date.Month;
            int a = date.Year;
            newEvents.Add(new Calendar { Title = "Réunion", Date = new DateTime(a, m, j, 10, 30, 0), user_Id = userId});
            Console.WriteLine(j);
        }
        
        ViewBag.CurrentYear = currentYear;
        ViewBag.CurrentMonth = currentMonth;
        ViewBag.CurrentWeek = currentWeek;
        ViewBag.StartOfWeek = currentDay;
        ViewBag.Events = events;
        ViewBag.NewEvents = newEvents;
        
        return View();
    }
    
    
    private int GetWeekOfMonth2(DateTime date)
    {
        DateTime firstDay = new DateTime(date.Year, date.Month, 1);
        return (date.Day + (int)firstDay.DayOfWeek - 1) / 7 + 1;
    }
    
    
    static List<string> GetPatientEventsNew(SqliteConnection connection, int? patientId)
    {
        var query = "SELECT date FROM appointment WHERE (patient_id,valid) = (@patient,@letter)";

        using SqliteCommand command = new SqliteCommand(query, connection);
        command.Parameters.AddWithValue("@patient", patientId);
        command.Parameters.AddWithValue("@letter", "N");
        connection.Open();
        using SqliteDataReader reader = command.ExecuteReader();

        List<string> dates = new List<string>();
        while (reader.Read())
        {
            dates.Add(reader["date"].ToString());
        }

        connection.Close();

        if (dates.Count > 0)
        {
            Console.WriteLine(" a accepter Rendez-vous trouvés : " + string.Join(", ", dates));
        }
        else
        {
            Console.WriteLine("Aucun rendez-vous trouvé.");
        }
        return dates;
    }

    static List<string> GetPatientEvents(SqliteConnection connection, int? patientId)
    {
        var query = "SELECT date FROM appointment WHERE (patient_id,valid) = (@patient,@letter)";

        using SqliteCommand command = new SqliteCommand(query, connection);
        command.Parameters.AddWithValue("@patient", patientId);
        command.Parameters.AddWithValue("@letter", "A");
        connection.Open();
        using SqliteDataReader reader = command.ExecuteReader();

        List<string> dates = new List<string>();
        while (reader.Read())
        {
            dates.Add(reader["date"].ToString());
        }

        connection.Close();

        if (dates.Count > 0)
        {
            Console.WriteLine("prevu Rendez-vous trouvés : " + string.Join(", ", dates));
        }
        else
        {
            Console.WriteLine("Aucun rendez-vous trouvé.");
        }
        return dates;
    }
    
    [HttpPost]
    public IActionResult AcceptEvent(int id)
    {
        var eventToAccept = _context.Appointment.FirstOrDefault(e => e.patient_id == id);
        if (eventToAccept != null)
        {
            eventToAccept.valid = "A"; // Suppose que tu as un champ Status
            _context.SaveChanges();
        }

        return RedirectToAction("HomeDoctor"); // Redirection après action
    }

    [HttpPost]
    public IActionResult DeclineEvent(int id)
    {
        var eventToDecline = _context.Appointment.FirstOrDefault(e => e.patient_id == id);
        if (eventToDecline != null)
        {
            eventToDecline.valid = "D"; 
            _context.SaveChanges();
        }

        return RedirectToAction("HomeDoctor"); 
    }

    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}