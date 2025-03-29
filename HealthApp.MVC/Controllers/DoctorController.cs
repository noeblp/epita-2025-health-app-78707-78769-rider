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
            

            
        List<(string,string,int,int)> res;
        List<(string,string,int,int)> res2;
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
            (string,string,int,int) dateStr = c; 
            DateTime date = DateTime.ParseExact(dateStr.Item1, "dd/MM/yyyy", null);
            int j = date.Day;
            int m = date.Month;
            int a = date.Year;
            DateTime jour = DateTime.ParseExact(dateStr.Item2, "HH:mm", null);
            int h = jour.Hour;
            int ms = jour.Minute;
            string user_name = _context.Users.FirstOrDefault(e=>e.user_id==dateStr.Item3).user_last_name;
            string firstName= _context.Users.FirstOrDefault(e=>e.user_id==dateStr.Item3).user_first_name;
            
            events.Add(new Calendar { Title = firstName+ " "+ user_name, Date = new DateTime(a, m, j, h, ms, 0), user_Id = userId,appo_id =dateStr.Item4 });
            
        }
        foreach (var c in res2)
        {
            (string,string,int,int) dateStr = c; 
            DateTime date = DateTime.ParseExact(dateStr.Item1, "dd/MM/yyyy", null);
            int j = date.Day;
            int m = date.Month;
            int a = date.Year;
            DateTime jour = DateTime.ParseExact(dateStr.Item2, "HH:mm", null);
            int h = jour.Hour;
            int ms = jour.Minute;
            string user_name = _context.Users.FirstOrDefault(e=>e.user_id==dateStr.Item3).user_last_name;
            string firstName= _context.Users.FirstOrDefault(e=>e.user_id==dateStr.Item3).user_first_name;
            newEvents.Add(new Calendar { Title = firstName+ " "+ user_name, Date = new DateTime(a, m, j, h, ms, 0), user_Id = userId,appo_id = dateStr.Item4 });
            
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
    
    

    static List<(string,string,int,int)> GetPatientEvents(SqliteConnection connection, int? patientId, string letter)
    {
        var query = "SELECT date,hour,name,appo_id,patient_id FROM appointment WHERE (doctor_id,valid) = (@patient,@letter)";

        using SqliteCommand command = new SqliteCommand(query, connection);
        command.Parameters.AddWithValue("@patient", patientId);
        command.Parameters.AddWithValue("@letter", letter);
        connection.Open();
        
        using SqliteDataReader reader = command.ExecuteReader();
        List<(string,string,int,int)> dates = new List<(string,string,int,int)>();
        string date = "";
        string hour = "";
        int name;
        int appo_id = 0;
        while (reader.Read())
        {
            date=reader["date"].ToString();
            hour = reader["hour"].ToString();
            name = int.Parse(reader["patient_id"].ToString());
            appo_id = int.Parse(reader["appo_id"].ToString());
            dates.Add((date,hour,name,appo_id));
        }
        connection.Close();
        
        return dates;
    }
    
    
    
    [HttpPost]
    public IActionResult AcceptEvent(int id)
    {
        var appo = _context.Appointment.Where(a => a.appo_id == id).FirstOrDefault();
        int maxId = _context.Notifications.Max(u => u.notif_id);


        _context.Notifications.Add(new Notification { notif_id = maxId+1, patient_id = appo.doctor_id, content = "The appointment on " +appo.date +"  at "+ appo.hour+" has been accepted." });
        _context.SaveChanges();
    
        string sql = "UPDATE Appointment SET valid = 'A' WHERE appo_id = @p0";
        _context.Database.ExecuteSqlRaw(sql, id);

        return RedirectToAction("HomeDoctor");
    }

    [HttpPost]
    public IActionResult DeclineEvent(int id)
    {
        var appo = _context.Appointment.Where(a => a.appo_id == id).FirstOrDefault();
        int maxId = _context.Notifications.Max(u => u.notif_id);


        _context.Notifications.Add(new Notification { notif_id = maxId+1, patient_id = appo.doctor_id, content = "The appointment on " +appo.date +"  at "+ appo.hour+" has been declined." });
        _context.SaveChanges();
    
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