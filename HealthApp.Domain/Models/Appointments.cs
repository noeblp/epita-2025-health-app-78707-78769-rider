using System.ComponentModel.DataAnnotations;

namespace hospital.Models;

public class Appointments
{
    [Key]
    public int appo_id { get; set; }
    public string doctor_id { get; set; }
    public string patient_id { get; set; }
    public string date { get; set; }
    public string valid { get; set; }
    public string hour { get; set; }
    
    public string name { get; set; }
    
    
}