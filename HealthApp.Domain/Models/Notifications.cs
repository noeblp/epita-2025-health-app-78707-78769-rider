using System.ComponentModel.DataAnnotations;

namespace hospital.Models;

public class Notifications
{
    [Key]
    public int notif_id { get; set; }
    public string patient_id { get; set; }
    public string content { get; set; }
}