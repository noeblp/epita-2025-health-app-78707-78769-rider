using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HealthApp.Razor.Data;
using Microsoft.AspNetCore.Mvc;
using hospital.Models;
using hospital.Modif_data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Calendar = hospital.Models.Calendar;
using EntityState = Microsoft.EntityFrameworkCore.EntityState;


namespace hospital.Controllers;

public class DoctorController : Controller
{
   
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;


    public DoctorController( ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        
        _context = context;
        _userManager = userManager;
        _context.Database.EnsureCreated();
        
        
    }
    
    

    public async Task<IActionResult> HomeDoctor(int? year, int? month, int? week, int? day)
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
        string email = HttpContext.Session.GetString("user_email");
        var user = await _userManager.FindByEmailAsync(email);
            res =GetPatientEvents(user.Id,"A");
            res2=GetPatientEvents(user.Id,"N");
        


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
            string user_name = _context.Users.FirstOrDefault(e=>e.user_id==dateStr.Item3).user_last_name;
            string firstName= _context.Users.FirstOrDefault(e=>e.user_id==dateStr.Item3).user_first_name;
            
            events.Add(new Calendar { Title = firstName+ " "+ user_name, Date = new DateTime(a, m, j, h, ms, 0), user_Id = user.Id,appo_id =dateStr.Item4 });
            
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
            string user_name = _context.Users.FirstOrDefault(e=>e.user_id==dateStr.Item3).user_last_name;
            string firstName= _context.Users.FirstOrDefault(e=>e.user_id==dateStr.Item3).user_first_name;
            newEvents.Add(new Calendar { Title = firstName+ " "+ user_name, Date = new DateTime(a, m, j, h, ms, 0), user_Id = user.Id,appo_id = dateStr.Item4 });
            
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
    
    

    public List<(string,string,string,int)> GetPatientEvents(string patientId, string letter)
    {
        var appointments = _context.Appointment
            .Where(a => a.doctor_id == patientId && a.valid == letter) // Filter by doctorId and valid
            .Select(a => new
            {
                a.date,
                a.hour,
                a.name,
                a.appo_id,
                a.patient_id
            })
            .ToList();

        var result = appointments.Select(a => (a.date, a.hour, a.name, a.appo_id)).ToList();
        
        return result;
    }
    
    
    
    [HttpPost]
    public IActionResult AcceptEvent(int id)
    {
        var appo = _context.Appointment.Where(a => a.appo_id == id).FirstOrDefault();
        int maxId = _context.Notifications.Max(u => u.notif_id);
        var email=_context.Users.FirstOrDefault(e=>e.user_id==appo.patient_id).user_email;
        SendMail.SendConfirmationEmail(email, "Appointment accepted", "Your appointment on " +appo.date +" at "+ appo.hour+" has been accepted.");

        _context.Notifications.Add(new Notifications { notif_id = maxId+1, patient_id = appo.doctor_id, content = "The appointment on " +appo.date +"  at "+ appo.hour+" has been accepted." });
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
        var email=_context.Users.FirstOrDefault(e=>e.user_id==appo.patient_id).user_email;
        SendMail.SendConfirmationEmail(email, "Appointment declined", "Your appointment on " +appo.date +" at "+ appo.hour+" has been declined.");

        _context.Notifications.Add(new Notifications { notif_id = maxId+1, patient_id = appo.doctor_id, content = "The appointment on " +appo.date +"  at "+ appo.hour+" has been declined." });
        _context.SaveChanges();
    
        string sql = "UPDATE Appointment SET valid = 'D' WHERE appo_id = @p0";
        _context.Database.ExecuteSqlRaw(sql, id);

        return RedirectToAction("HomeDoctor"); 
    }

    

    
}