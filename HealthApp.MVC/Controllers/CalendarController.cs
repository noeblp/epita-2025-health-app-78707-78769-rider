using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using HealthApp.Razor.Data;
using hospital.Models;
using hospital.Modif_data;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Calendar = hospital.Models.Calendar;

namespace hospital.Controllers
{
    public class CalendarController : Controller
    {
        
        private readonly ApplicationDbContext _context;
    

        public CalendarController( ApplicationDbContext context)
        {
        
            _context = context;
            _context.Database.EnsureCreated();
        
        
        }
        public IActionResult Calendar(int? year, int? month, int? week)
        {
            int currentYear = year ?? DateTime.Now.Year;
            int currentMonth = month ?? DateTime.Now.Month;

            DateTime firstDayOfMonth = new DateTime(currentYear, currentMonth, 1);
            int currentWeek = week ?? GetWeekOfMonth(DateTime.Now);

            var firstMonday = firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek + 1);
            if (firstMonday > firstDayOfMonth) firstMonday = firstMonday.AddDays(-7);

            var startOfWeek = firstMonday.AddDays((currentWeek - 1) * 7);
            

            
            List<(string,string,int)> res;
            int? userId = HttpContext.Session.GetInt32("user_id");
            using (var connection = ModifUser.ConnectToDatabase())
            {
                res =GetPatientEvents(connection,userId);
            }


            var events = new List<Calendar>();

            foreach (var c in res)
            {
                (string,string,int) dateStr = c; 
                DateTime date = DateTime.ParseExact(dateStr.Item1, "dd/MM/yyyy", null);
                int j = date.Day;
                int m = date.Month;
                int a = date.Year;
                DateTime jour = DateTime.ParseExact(dateStr.Item2, "HH:mm", null);
                int h = jour.Hour;
                int ms = jour.Minute;

                string user_name = _context.Users.FirstOrDefault(e=>e.user_id==dateStr.Item3).user_last_name;
                string firstName= _context.Users.FirstOrDefault(e=>e.user_id==dateStr.Item3).user_first_name;
                events.Add(new Calendar { Title = firstName+ " "+ user_name, Date = new DateTime(a, m, j, h, ms, 0), user_Id = userId});
                
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

        


        static List<(string,string,int)> GetPatientEvents(SqliteConnection connection, int? patientId)
        {
            var query = "SELECT date,hour,patient_id FROM appointment WHERE (doctor_id,valid) = (@patient,@letter)";

            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@patient", patientId);
            command.Parameters.AddWithValue("@letter", "A");
            connection.Open();
        
            using SqliteDataReader reader = command.ExecuteReader();
            List<(string,string,int)> dates = new List<(string,string,int)>();
            string date = "";
            string hour = "";
            int name;
            while (reader.Read())
            {
                date=reader["date"].ToString();
                hour = reader["hour"].ToString();
                name = int.Parse(reader["patient_id"].ToString());
                dates.Add((date,hour,name));
            }
            connection.Close();
            
            return dates;
        }

    }
}