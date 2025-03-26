using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using Microsoft.EntityFrameworkCore;
using Calendar = hospital.Models.Calendar;
using EntityState = Microsoft.EntityFrameworkCore.EntityState;


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
            

            
        List<(string,string,string,int)> res;
        List<(string,string,string,int)> res2;
        int? userId = HttpContext.Session.GetInt32("user_id");
        using (var connection = ModifUser.ConnectToDatabase())
        {
            res =GetPatientEvents(connection,userId,"A");
            res2=GetPatientEvents(connection,userId,"N");
        }


        var events = new List<Calendar>();
        var newEvents = new List<Calendar>();

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
            
            events.Add(new Calendar { Title = dateStr.Item3, Date = new DateTime(a, m, j, h, ms, 0), user_Id = userId,appo_id =dateStr.Item4 });
            
        }
        foreach (var c in res2)
        {
            (string,string,string,int) dateStr = c; 
            DateTime date = DateTime.ParseExact(dateStr.Item1, "dd/MM/yyyy", null);
            int j = date.Day;
            int m = date.Month;
            int a = date.Year;
            DateTime jour = DateTime.ParseExact(dateStr.Item2, "HH:mm", null);
            int h = jour.Hour;
            int ms = jour.Minute;
            newEvents.Add(new Calendar { Title = dateStr.Item3, Date = new DateTime(a, m, j, h, ms, 0), user_Id = userId,appo_id = dateStr.Item4 });
            
        }
        
        ViewBag.CurrentYear = currentYear;
        ViewBag.CurrentMonth = currentMonth;
        ViewBag.CurrentWeek = currentWeek;
        ViewBag.StartOfWeek = currentDay;
        ViewBag.Events = null;
        ViewBag.Events = events;
        ViewBag.NewEvents = newEvents;
        
        return View();
    }
    
    
    private int GetWeekOfMonth2(DateTime date)
    {
        DateTime firstDay = new DateTime(date.Year, date.Month, 1);
        return (date.Day + (int)firstDay.DayOfWeek - 1) / 7 + 1;
    }
    
    

    static List<(string,string,string,int)> GetPatientEvents(SqliteConnection connection, int? patientId, string letter)
    {
        var query = "SELECT date,hour,name,appo_id FROM appointment WHERE (doctor_id,valid) = (@patient,@letter)";

        using SqliteCommand command = new SqliteCommand(query, connection);
        command.Parameters.AddWithValue("@patient", patientId);
        command.Parameters.AddWithValue("@letter", letter);
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
    
    
    
    [HttpPost]
    public IActionResult AcceptEvent(int id)
    {
        Console.WriteLine("id " + id);
    
        string sql = "UPDATE Appointment SET valid = 'A' WHERE appo_id = @p0";
        _context.Database.ExecuteSqlRaw(sql, id);

        return RedirectToAction("HomeDoctor");
    }

    [HttpPost]
    public IActionResult DeclineEvent(int id)
    {
        Console.WriteLine("id " + id);
    
        string sql = "UPDATE Appointment SET valid = 'D' WHERE appo_id = @p0";
        _context.Database.ExecuteSqlRaw(sql, id);

        return RedirectToAction("HomeDoctor"); 
    }

    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}