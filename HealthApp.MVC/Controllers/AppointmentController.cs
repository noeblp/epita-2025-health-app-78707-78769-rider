using System;
using System.Collections.Generic;
using System.Linq;
using HealthApp.Razor.Data;
using hospital.Models;
using hospital.Modif_data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Appointments = HealthApp.Razor.Data.Appointments;

namespace hospital.Controllers;

public class AppointmentController:Controller
{
    
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    

    public AppointmentController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
        _context.Database.EnsureCreated();
        
        
    }
    public IActionResult BookAppo(int? year, int? month, int? week)
    {
        if (TempData["SelectedHour"]!=null){ViewBag.SelectedHour = HttpContext.Session.GetString("SelectedHour")+":00";}
        if (TempData["SelectedHour"] != null)
        {
            ViewBag.SelectedDate = HttpContext.Session.GetString("SelectedDate") + "/" +
                                   HttpContext.Session.GetString("SelectedMonth") + "/" +
                                   HttpContext.Session.GetString("SelectedYear");

        }

       
        int currentYear = year ?? DateTime.Now.Year;
        int currentMonth = month ?? DateTime.Now.Month;

        DateTime firstDayOfMonth = new DateTime(currentYear, currentMonth, 1);
        int currentWeek = week ?? GetWeekOfMonth(DateTime.Now);

        var firstMonday = firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek + 1);
        if (firstMonday > firstDayOfMonth) firstMonday = firstMonday.AddDays(-7);

        var startOfWeek = firstMonday.AddDays((currentWeek - 1) * 7);
            

            
        List<(string,string,string,int)> res;
        int? userId = HttpContext.Session.GetInt32("user_id");
        using (var connection = ModifUser.ConnectToDatabase())
        {
            res =GetPatientEvents(connection,userId);
        }


        var events = new List<Calendar>();

        foreach (var c in res)
        {
            (string,string,string,int) dateStr = c; 
            DateTime date = DateTime.ParseExact(dateStr.Item1, "dd/MM/yyyy", null);
            int j = date.Day;
            int m = date.Month;
            int a = date.Year;
            DateTime jour = DateTime.ParseExact(dateStr.Item2, "HH:mm", null);
            int h = jour.Hour;
            int ms = jour.Minute;
            
            events.Add(new Calendar { Title = dateStr.Item3, Date = new DateTime(a, m, j, h, ms, 0), user_Id = userId,appo_id = dateStr.Item4});
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
    
    static List<(string,string,string,int)> GetPatientEvents(SqliteConnection connection, int? patientId)
    {
        var query = "SELECT date,hour,name,appo_id FROM appointment WHERE (patient_id,valid) = (@patient,@letter)";

        using SqliteCommand command = new SqliteCommand(query, connection);
        command.Parameters.AddWithValue("@patient", patientId);
        command.Parameters.AddWithValue("@letter", "A");
        connection.Open();
        
        using SqliteDataReader reader = command.ExecuteReader();
        List<(string,string,string,int)> dates = new List<(string,string,string,int)>();
        string date = "";
        string hour = "";
        string name = "";
        int appo_id = 0;
        while (reader.Read())
        {
            date=reader["date"].ToString();
            hour = reader["hour"].ToString();
            name = reader["name"].ToString();
            appo_id = int.Parse(reader["appo_id"].ToString());
            dates.Add((date,hour,name,appo_id));
        }
        connection.Close();
        
        return dates;
    }

    private string hours;

    public IActionResult Popup(int hour, int date, int month,int year)
    {
        TempData["SelectedHour"] = hour.ToString();
        TempData["SelectedDate"] = date.ToString();
        TempData["SelectedMonth"] = month.ToString();
        TempData["SelectedYear"] = year.ToString();
        return View();
    }
    
    
    [HttpPost]
    public IActionResult SubmitAppo(string date, string name,string hour)
    {
        string? hours = HttpContext.Session.GetString("SelectedHour");
        string? dates = TempData["SelectedDate"] as string;
        string? months = TempData["SelectedMonth"] as string;
        string? years = TempData["SelectedYear"] as string;
        
        if (!string.IsNullOrEmpty(months))
        {
            months = int.Parse(months).ToString("D2");
        }
        if (!string.IsNullOrEmpty(hours))
        {
            hours = int.Parse(hours).ToString("D2");
        }
        
        string? final_date=dates+"/"+months+"/"+years;
        
        
        int max =_context.Appointment.Max(a=>a.appo_id);
        
        _context.Appointment.Add(new Appointments
            { doctor_id = "marche", patient_id = 8, date = final_date ,valid = "N", hour= hours+":00" ,name = name,appo_id = max+1});
        _context.SaveChanges();
        
        
        return RedirectToAction("BookAppo");
    }

    
    public IActionResult Init(int hour, int date, int month,int year)
    {
        
        TempData["SelectedHour"] = hour.ToString();
        TempData["SelectedDate"] = date.ToString();
        TempData["SelectedMonth"] = month.ToString();
        TempData["SelectedYear"] = year.ToString();
        HttpContext.Session.SetString("SelectedHour", TempData["SelectedHour"]?.ToString());
        HttpContext.Session.SetString("SelectedDate", TempData["SelectedDate"]?.ToString());
        HttpContext.Session.SetString("SelectedMonth", TempData["SelectedMonth"]?.ToString());
        HttpContext.Session.SetString("SelectedYear", TempData["SelectedYear"]?.ToString());

        return RedirectToAction("BookAppo");    
    }
    
    
}