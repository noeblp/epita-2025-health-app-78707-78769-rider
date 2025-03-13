using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using hospital.Models;
using Calendar = hospital.Models.Calendar;

namespace hospital.Controllers
{
    public class CalendarController : Controller
    {
        public IActionResult Index(int? year, int? month, int? week)
        {
            int currentYear = year ?? DateTime.Now.Year;
            int currentMonth = month ?? DateTime.Now.Month;

            // Calcul de la semaine actuelle si elle n'est pas définie
            DateTime firstDayOfMonth = new DateTime(currentYear, currentMonth, 1);
            int currentWeek = week ?? GetWeekOfMonth(DateTime.Now);

            // Calcul du premier lundi du mois
            var firstMonday = firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek + 1);
            if (firstMonday > firstDayOfMonth) firstMonday = firstMonday.AddDays(-7);

            // Déterminer la date de début de la semaine
            var startOfWeek = firstMonday.AddDays((currentWeek - 1) * 7);

            // Liste d'événements fictifs (remplace par une BDD si nécessaire)
            var events = new List<Calendar>
            {
                //new Calendar { Title = "Réunion", Date = new DateTime(currentYear, currentMonth, 5) },
                //new Calendar { Title = "Conférence", Date = new DateTime(currentYear, currentMonth, 15) }
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
    }
}