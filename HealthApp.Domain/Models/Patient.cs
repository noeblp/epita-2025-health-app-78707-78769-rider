using System.ComponentModel.DataAnnotations;

namespace hospital.Models;

public class Patient
{
    [Key]
    public string patient_id { get; set; }
    public string patient_name { get; set; }
    public string patient_last_name { get; set; }
    public string patient_email { get; set; }
}