using System;

namespace hospital.Models
{
    public class Calendar
    {
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public int? user_Id { get; set; }
        
        
        public string Status { get; set; }
        
        public int appo_id { get; set; }
    }
}