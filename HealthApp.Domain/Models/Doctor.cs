using System.ComponentModel.DataAnnotations;

namespace hospital.Models;

public class Doctor
{
    [Key]
    public string doctor_id { get; set; }
    public string doctor_first_name { get; set; }
    public string doctor_last_name { get; set; }
    public string doctor_email { get; set; }
    public string doctor_specialty { get; set; }
}