using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using HealthApp.Razor.Data;
using hospital.Models;
using hospital.Modif_data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Calendar = hospital.Models.Calendar;

namespace hospital.Controllers
{
    public class CalendarController : Controller
    {
        
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
    

        public CalendarController( ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
        
            _context = context;
            _userManager = userManager;
            _context.Database.EnsureCreated();
        
        
        }
        public async Task<IActionResult> Calendar(int? year, int? month, int? week)
        {
            int currentYear = year ?? DateTime.Now.Year;
            int currentMonth = month ?? DateTime.Now.Month;

            DateTime firstDayOfMonth = new DateTime(currentYear, currentMonth, 1);
            int currentWeek = week ?? GetWeekOfMonth(DateTime.Now);

            var firstMonday = firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek + 1);
            if (firstMonday > firstDayOfMonth) firstMonday = firstMonday.AddDays(-7);

            var startOfWeek = firstMonday.AddDays((currentWeek - 1) * 7);
            

            
            List<(string,string,string)> res;
            string email = HttpContext.Session.GetString("user_email");
            var user = await _userManager.FindByEmailAsync(email);
                res =GetPatientEvents(user.Id);
            


            var events = new List<Calendar>();

            foreach (var c in res)
            {
                (string,string,string) dateStr = c; 
                DateTime date = DateTime.ParseExact(dateStr.Item1, "dd/MM/yyyy", null);
                int j = date.Day;
                int m = date.Month;
                int a = date.Year;
                DateTime jour = DateTime.ParseExact(dateStr.Item2, "HH:mm", null);
                int h = jour.Hour;
                int ms = jour.Minute;

                string user_name = _context.Users.FirstOrDefault(e=>e.user_id==dateStr.Item3).user_last_name;
                string firstName= _context.Users.FirstOrDefault(e=>e.user_id==dateStr.Item3).user_first_name;
                events.Add(new Calendar { Title = firstName+ " "+ user_name, Date = new DateTime(a, m, j, h, ms, 0), user_Id = user.Id});
                
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

        


        public List<(string,string,string)> GetPatientEvents(string patientId)
        {
            var appointments = _context.Appointment
                .Where(a => a.doctor_id == patientId && a.valid == "A") // Filter by doctorId and valid
                .Select(a => new
                {
                    a.date,
                    a.hour,
                    a.patient_id
                })
                .ToList();

            var result = appointments.Select(a => (a.date, a.hour, a.patient_id.ToString())).ToList();

            return result;
        }

    }
}