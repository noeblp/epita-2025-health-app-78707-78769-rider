using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using HealthApp.Razor.Data;
using hospital.Models;
using hospital.Models.User;
using hospital.Modif_data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;


namespace hospital.Controllers;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly string _connectionString = "Data Source=hospital.db";
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;


    public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {

        _context = context;
        _context.Database.EnsureCreated();
        _userManager = userManager;
        _signInManager = signInManager;
        

    }

    
    


    public IActionResult AddDoctor()
    {
        return View();
    }

    

    [HttpPost]
    public IActionResult Add(string firstname, string lastname, string email, string password, string specialty)
    {
        using (var connection = modif_doctors.ConnectToDatabase())
        {
            modif_doctors.InsertDoctors(connection, firstname, lastname, email, password, specialty);
            return RedirectToAction("AddDoctor");
        }
        // return RedirectToAction("Index", "Home");

    }

    public IActionResult UserList()
    {
        
        var users= _context.Users.ToList();
        ViewBag.Users = users;
        return View(users);
    }

    [HttpPost]
    public IActionResult UpdateUser(int user_id, string user_first_name, string user_last_name, string user_email,
        string user_role)
    {
        if (string.IsNullOrEmpty(user_first_name) ||
            string.IsNullOrEmpty(user_last_name) ||
            string.IsNullOrEmpty(user_email) ||
            string.IsNullOrEmpty(user_role))
        {
            ModelState.AddModelError(string.Empty, "All fields are required.");
            return View("EditUser");
        }

        using (var connection = ModifUser.ConnectToDatabase())
        {
            connection.Open();
            using (var command =
                   new SqliteCommand(
                       "UPDATE users SET user_first_name = @firstName, user_last_name = @lastName, user_email = @email, user_role = @role WHERE user_id = @id",
                       connection))
            {
                command.Parameters.AddWithValue("@firstName", user_first_name);
                command.Parameters.AddWithValue("@lastName", user_last_name);
                command.Parameters.AddWithValue("@email", user_email);
                command.Parameters.AddWithValue("@role", user_role);
                command.Parameters.AddWithValue("@id", user_id);

                command.ExecuteNonQuery();
            }

            connection.Close();
        }

        return RedirectToAction("UI_admin","Home");
    }

    public IActionResult EditDoctor(string id)
    {
        var doc=_context.Doctors.FirstOrDefault(e=>e.doctor_id==id);
        return View(doc);
    }

    
  
    public async Task<IActionResult> EditUser(string userid)
    {
        Console.WriteLine($"User ID reçu: {userid}");
        var user = _context.Users.FirstOrDefault(a => a.user_id == userid);
        
        
        var test = await _userManager.FindByIdAsync(user.user_id);
        var roles = await _userManager.GetRolesAsync(test);
        string role = roles.Count > 0 ? roles[0] : "Unknown";
        Console.WriteLine("Role: "+role);
        if (role == "DOCTOR")
        {
            return RedirectToAction("EditDoctor", "Admin", new {id = user.user_id});
        }
        
        return View(_context.Patient.FirstOrDefault(e=>e.patient_id==user.user_id));
        
        
        
    }



    [HttpGet]
    public IActionResult AddUser()
    {
        return View();
    }
    

        [HttpPost]
        public async Task<IActionResult> AddUser(string user_first_name, string user_last_name, string user_email,
            string user_password,string specialty)
        {
            var user = new IdentityUser { UserName = user_last_name, Email = user_email};
            var result = await _userManager.CreateAsync(user, user_password);


            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "DOCTOR");

                _context.Users.Add(new User
                {
                    user_email = user.Email,
                    user_id = user.Id,
                    user_last_name = user_last_name,
                    user_first_name = user_first_name,
                    user_password = user_password,
                    user_role = "D"
                });
                await _context.SaveChangesAsync();

                _context.Doctors.Add(new Doctor
                {
                    doctor_email = user.Email,
                    doctor_id = user.Id,
                    doctor_last_name = user.UserName,
                    doctor_specialty = specialty,
                    doctor_first_name = user.UserName
                });
                await _context.SaveChangesAsync();
            }

            //####################################CHANGE REDIRECTION###########################################
            return RedirectToAction("UI_admin","Home"); // Redirect back to the user list
    }
        
        
        
        
        public IActionResult Manage()
        {
            var rdvs = _context.Appointment.ToList();
            return View(rdvs);
        }

        public IActionResult Delete(int id)
        {
            _context.Appointment.FirstOrDefault(e=>e.appo_id==id)!.valid="C";
            _context.SaveChanges();
            
            
            return RedirectToAction("Manage");
        }

        

        public IActionResult Edit(int id)
        {
            var rdv = _context.Appointment.FirstOrDefault(e=>e.appo_id==id);
            if (rdv == null)
            {
                return NotFound();
            }
            return View(rdv);
        }
        
    
}


