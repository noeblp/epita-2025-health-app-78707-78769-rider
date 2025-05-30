using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HealthApp.Razor.Data;
using hospital.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Calendar = hospital.Models.Calendar;

namespace hospital.Controllers;

public class AppointmentController:Controller
{
    
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    

    public AppointmentController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _context.Database.EnsureCreated();
        
        
    }
    public IActionResult BookAppo(int? year, int? month, int? week, string doctorId)
    {
        if (TempData["SelectedHour"]!=null){ViewBag.SelectedHour = HttpContext.Session.GetString("SelectedHour")+":00";}
        if (TempData["SelectedHour"] != null)
        {
            ViewBag.SelectedDate = HttpContext.Session.GetString("SelectedDate") + "/" +
                                   HttpContext.Session.GetString("SelectedMonth") + "/" +
                                   HttpContext.Session.GetString("SelectedYear");
        }
        if (HttpContext.Session.GetString("doctor_id") == null)
        {
            HttpContext.Session.SetString("doctor_id",doctorId.ToString());
        }
        
        var doc = _context.Doctors.FirstOrDefault(e => e.doctor_id == HttpContext.Session.GetString("doctor_id"));
        
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
        
        var user_email=HttpContext.Session.GetString("user_email");
        var user= _context.Users.FirstOrDefault(e=>e.user_email==user_email);
        res =GetPatientEvents(doctorId);
        
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
            
            events.Add(new Calendar { Title = dateStr.Item3, Date = new DateTime(a, m, j, h, ms, 0), user_Id = user.user_id,appo_id = dateStr.Item4});
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
    
    public List<(string,string,string,int)> GetPatientEvents(string patientId)
    {
        var appointments = _context.Appointment
            .Where(a => a.doctor_id == patientId && (a.valid == "A" || a.valid=="N"))
            .Select(a => new { a.date, a.hour, a.name, a.appo_id })
            .ToList();
    
        return appointments.Select(a => (a.date, a.hour, a.name, a.appo_id)).ToList();
    }
    
    
    [HttpPost]
    public IActionResult SubmitAppo(string date, string name,string hour)
    {
        string doctorid = HttpContext.Session.GetString("doctor_id");
        var user_email=HttpContext.Session.GetString("user_email");
        var user= _context.Users.FirstOrDefault(e=>e.user_email==user_email);
        
        string? hours = HttpContext.Session.GetString("SelectedHour");
        string? dates = TempData["SelectedDate"] as string;
        string? months = TempData["SelectedMonth"] as string;
        string? years = TempData["SelectedYear"] as string;
        if (!string.IsNullOrEmpty(dates))
        {
            dates = int.Parse(dates).ToString("D2");
        }
        
        if (!string.IsNullOrEmpty(months))
        {
            months = int.Parse(months).ToString("D2");
        }
        if (!string.IsNullOrEmpty(hours))
        {
            hours = int.Parse(hours).ToString("D2");
        }
        
        string? final_date=dates+"/"+months+"/"+years;
        var test=_context.Appointment.FirstOrDefault(e=>e.patient_id==user.user_id && e.date==final_date && e.hour==hour);
        if (test != null)
        {
            TempData["AlertMessage"] = "You have already an appointment at this time";
        }


        int max =_context.Appointment.Max(a=>a.appo_id);
        
        _context.Appointment.Add(new Appointments
            { doctor_id = doctorid, patient_id = user.user_id, date = final_date ,valid = "N", hour= hours+":00" ,name = name,appo_id = max+1});
        _context.SaveChanges();
        
        
        
        return RedirectToAction("BookAppo","Appointment",new { doctorid = doctorid });
    }

    
    public IActionResult Init(int hour, int date, int month,int year)
    {
        
        TempData["SelectedHour"] = hour.ToString();
        TempData["SelectedDate"] = date.ToString(new CultureInfo("en-US"));
        TempData["SelectedMonth"] = month.ToString(new CultureInfo("en-US"));
        TempData["SelectedYear"] = year.ToString(new CultureInfo("en-US"));
        HttpContext.Session.SetString("SelectedHour", TempData["SelectedHour"]?.ToString());
        HttpContext.Session.SetString("SelectedDate", TempData["SelectedDate"]?.ToString());
        HttpContext.Session.SetString("SelectedMonth", TempData["SelectedMonth"]?.ToString());
        HttpContext.Session.SetString("SelectedYear", TempData["SelectedYear"]?.ToString());
        //change
        return RedirectToAction("BookAppo");    
    }



    public IActionResult FuturAppo(string doctorFilter, string dateFilter, string statusFilter)
    {
        var user_email=HttpContext.Session.GetString("user_email");
        var user= _context.Users.FirstOrDefault(e=>e.user_email==user_email);
        var appointments = _context.Appointment
            .Where(a => a.patient_id == user.user_id)
            .Select(a => new { a.date, a.hour, a.name, a.appo_id, a.doctor_id, a.valid })
            .ToList();
    
        var rdvs = new List<Calendar>();
        foreach (var a in appointments)
        {
            DateTime date = DateTime.ParseExact(a.date, "dd/MM/yyyy", null);
            DateTime hour = DateTime.ParseExact(a.hour, "HH:mm", null);

            var doctorLastName = _context.Doctors
                .Where(d => d.doctor_id == a.doctor_id)
                .Select(d => d.doctor_last_name)
                .FirstOrDefault() ?? " ";
            string stat;
            if (a.valid == "A") stat="valid";
            else if (a.valid=="N") stat="on hold";
            else stat="canceled";
            
            
        
            rdvs.Add(new Calendar
            {
                appo_id = a.appo_id,
                Date = new DateTime(date.Year, date.Month, date.Day, hour.Hour, hour.Minute, 0),
                Status = a.valid,
                stat = stat,
                Title = doctorLastName,
                user_Id = user.user_id,
            });
        }
        
    
        if (!string.IsNullOrEmpty(doctorFilter))
        {
            rdvs = rdvs.Where(r => r.Title == doctorFilter).ToList();
        }
    
        if (!string.IsNullOrEmpty(dateFilter) && DateTime.TryParse(dateFilter, out DateTime selectedDate))
        {
            rdvs = rdvs.Where(r => r.Date.Date == selectedDate.Date).ToList();
        }
    
        if (!string.IsNullOrEmpty(statusFilter))
        {
            rdvs = rdvs.Where(r => r.Status == statusFilter).ToList();
        }
        rdvs = rdvs.OrderBy(r => r.Date).ToList();
    
        ViewBag.SelectedDoctor = doctorFilter;
        ViewBag.SelectedDate = dateFilter;
        ViewBag.SelectedStatus = statusFilter;
        ViewBag.List = rdvs;
        return View(rdvs);
    }
    
    
    [HttpPost]
    public IActionResult CancelEvent(int id)
    {
        
        var appo = _context.Appointment.Where(a => a.appo_id == id).FirstOrDefault();
        int maxId = _context.Notifications.Max(u => u.notif_id);
        var email=_context.Users.FirstOrDefault(e=>e.user_id==appo.doctor_id).user_email;
        SendMail.SendConfirmationEmail(email, "Appointment canceled", "Your appointment on " +appo.date +" at "+ appo.hour+" has been canceled.");

        _context.Notifications.Add(new Notifications { notif_id = maxId+1, patient_id = appo.doctor_id, content = "The appointment on " +appo.date +"  at "+ appo.hour+" has been canceled." });
        _context.SaveChanges();
        
        string sql = "UPDATE Appointment SET valid = 'C' WHERE appo_id = @p0";
        _context.Database.ExecuteSqlRaw(sql, id);
        return RedirectToAction("FuturAppo");
    }
    
    
}