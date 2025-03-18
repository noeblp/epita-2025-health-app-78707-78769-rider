using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using hospital.Models;
using hospital.Modif_data;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Calendar = hospital.Models.Calendar;

namespace hospital.Controllers
{
    public class CalendarController : Controller
    {
        public IActionResult Calendar(int? year, int? month, int? week)
        {
            int currentYear = year ?? DateTime.Now.Year;
            int currentMonth = month ?? DateTime.Now.Month;

            DateTime firstDayOfMonth = new DateTime(currentYear, currentMonth, 1);
            int currentWeek = week ?? GetWeekOfMonth(DateTime.Now);

            var firstMonday = firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek + 1);
            if (firstMonday > firstDayOfMonth) firstMonday = firstMonday.AddDays(-7);

            var startOfWeek = firstMonday.AddDays((currentWeek - 1) * 7);
            HttpContext.Session.GetString("user_id");
            
            
            
            
            int patientId = 8; // Exemple d'ID patient
            using (var connection = ModifUser.ConnectToDatabase())
            {
                GetPatientEvents(connection,"8");
            }
            

            
            
            var events = new List<Calendar>
            {
                //new Calendar { Title = "Réunion", Date = new DateTime(currentYear, currentMonth, 18, 10, 30, 0) },
                //new Calendar { Title = "Conférence", Date = new DateTime(currentYear, currentMonth, 20, 14, 0, 0) }
            };

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


        static void GetPatientEvents(SqliteConnection connection,string patientId)
        {
            var query = "SELECT date FROM appointment WHERE patient_id = @patient_id";

            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@patient_id", 8);

            connection.Open();
            var result = command.ExecuteScalar();
            if (result != null)
            {
                Console.WriteLine("ee" + result);
            }
            else
            {
                Console.WriteLine("vide");
            }

            connection.Close();

            
        }

    }
}