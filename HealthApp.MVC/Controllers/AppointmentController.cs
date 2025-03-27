using System;
using System.Collections.Generic;
using System.Linq;
using HealthApp.Razor.Data;
using hospital.Models;
using hospital.Modif_data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
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
    public IActionResult BookAppo(int? year, int? month, int? week, int doctorId)
    {
        if (TempData["SelectedHour"]!=null){ViewBag.SelectedHour = HttpContext.Session.GetString("SelectedHour")+":00";}
        if (TempData["SelectedHour"] != null)
        {
            ViewBag.SelectedDate = HttpContext.Session.GetString("SelectedDate") + "/" +
                                   HttpContext.Session.GetString("SelectedMonth") + "/" +
                                   HttpContext.Session.GetString("SelectedYear");

        }

        if (HttpContext.Session.GetInt32("doctor_id") == null)
        {
            HttpContext.Session.SetInt32("doctor_id",doctorId);
        }
        var doc = _context.Doctors.FirstOrDefault(e => e.doctor_id == HttpContext.Session.GetInt32("doctor_id"));
        ViewBag.DoctorName = doc.doctor_last_name;
        ViewBag.DoctorSpecialty = doc.doctor_specialty;
        
        

       
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
            res =GetPatientEvents(connection,doctorId);
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
    
    public List<(string,string,string,int)> GetPatientEvents(SqliteConnection connection, int? patientId)
    {
        var appointments = _context.Appointment
            .Where(a => a.doctor_id == patientId && a.valid == "A")
            .Select(a => new { a.date, a.hour, a.name, a.appo_id })
            .ToList();
    
        return appointments.Select(a => (a.date, a.hour, a.name, a.appo_id)).ToList();
    }
    
    
    [HttpPost]
    public IActionResult SubmitAppo(string date, string name,string hour)
    {
        int? doctorid = HttpContext.Session.GetInt32("doctor_id");
        int? patient_id = HttpContext.Session.GetInt32("user_id");
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
            { doctor_id = doctorid, patient_id = patient_id, date = final_date ,valid = "N", hour= hours+":00" ,name = name,appo_id = max+1});
        _context.SaveChanges();
        
        
        
        return RedirectToAction("BookAppo","Appointment",new { doctorid = doctorid });
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



    public IActionResult FuturAppo()
    {
        
        int? doc_id=HttpContext.Session.GetInt32("user_id");
        Console.WriteLine("doc_id= "+doc_id);
        var appointments = _context.Appointment
            .Where(a => a.patient_id == doc_id && a.valid == "A")
            .Select(a => new { a.date, a.hour, a.name, a.appo_id,a.doctor_id })
            .ToList();
    
         appointments.Select(a => (a.date, a.hour, a.name, a.appo_id,a.doctor_id)).ToList();
        
         
         var rdvs = new List<Calendar>();
         foreach (var a in appointments)
         {
             Console.WriteLine("date = "+a.hour);
             DateTime date = DateTime.ParseExact(a.date, "dd/MM/yyyy", null);
             int j = date.Day;
             int m = date.Month;
             int y = date.Year;
             DateTime hour = DateTime.ParseExact(a.hour, "HH:mm", null);
             int h = hour.Hour;
             int ms = hour.Minute;


             var doctorLastName = _context.Doctors
                 .Where(d => d.doctor_id == a.doctor_id)
                 .Select(d => d.doctor_last_name)
                 .FirstOrDefault() ?? " ";
             
             rdvs.Add(new Calendar{appo_id = a.appo_id,Date = new DateTime(y,m,j,h,ms,0),Status = "V",Title = doctorLastName, user_Id = 12});
         } 
         
         
        
         
         
        ViewBag.List = rdvs;
        return View(rdvs);
    }
    
    
    [HttpPost]
    public IActionResult CancelEvent(int id)
    {
        Console.WriteLine("id " + id);
    
        string sql = "UPDATE Appointment SET valid = 'C' WHERE appo_id = @p0";
        _context.Database.ExecuteSqlRaw(sql, id);
        
        return RedirectToAction("FuturAppo");
    }
    
    
}