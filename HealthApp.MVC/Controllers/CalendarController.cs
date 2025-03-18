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
            



            List<string> res;
            int? userId = HttpContext.Session.GetInt32("user_id");
            using (var connection = ModifUser.ConnectToDatabase())
            {
                res =GetPatientEvents(connection,userId);
            }


            var events = new List<Calendar>();

            foreach (var c in res)
            {
                string dateStr = c; 
                DateTime date = DateTime.ParseExact(dateStr, "dd/MM/yyyy", null);

                int j = date.Day;
                int m = date.Month;
                int a = date.Year;
                events.Add(new Calendar { Title = "Réunion", Date = new DateTime(a, m, j, 10, 30, 0) });
                Console.WriteLine(j);
            }
            /*var events = new List<Calendar>
            {
                //new Calendar { Title = "Réunion", Date = new DateTime(currentYear, currentMonth, 18, 10, 30, 0) },
                //new Calendar { Title = "Conférence", Date = new DateTime(currentYear, currentMonth, 20, 14, 0, 0) }
            };*/

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


        static List<string> GetPatientEvents(SqliteConnection connection, int? patientId)
        {
            var query = "SELECT date FROM appointment WHERE patient_id = @patient";

            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@patient", patientId);
            Console.WriteLine(patientId);
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
                Console.WriteLine("Rendez-vous trouvés : " + string.Join(", ", dates));
            }
            else
            {
                Console.WriteLine("Aucun rendez-vous trouvé.");
            }
            return dates;
        }

    }
}