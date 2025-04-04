using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace hospital.Models.User;

public class User
{
    [Key]
    public string user_id { get; set; }
    public string user_first_name { get; set; }
    public string user_last_name { get; set; }
    public string user_email { get; set; }
    
}