using System.ComponentModel.DataAnnotations;

namespace hospital.Models;

public class Admin
{
    [Key]
    public string admin_id { get; set; }
    public string admin_email { get; set; }
}