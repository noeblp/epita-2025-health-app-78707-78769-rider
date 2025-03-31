using System.ComponentModel.DataAnnotations;

namespace hospital.Models;

public class Admin
{
    [Key]
    public int admin_id { get; set; }
}