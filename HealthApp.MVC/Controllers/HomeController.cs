using System.Data.SQLite;
using System.Diagnostics;
<<<<<<< Updated upstream
using System.Security.Claims;
=======
using System.Threading.Tasks;
>>>>>>> Stashed changes
using HealthApp.Razor.Data;
using Microsoft.AspNetCore.Mvc;
using hospital.Models;
using hospital.Modif_data;
<<<<<<< Updated upstream
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

=======
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Identity;
>>>>>>> Stashed changes

namespace hospital.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _context.Database.EnsureCreated();
    }

    public IActionResult Index()
    {
        
        return View();
    }

    
    public IActionResult Privacy()
    {
<<<<<<< Updated upstream
=======
        HttpContext.Session.Clear();
>>>>>>> Stashed changes
        //send_mail.SendConfirmationEmail("belperin.n@gmail.com","ravus","ravus");
        return View();
    }

    public IActionResult Register()
    {
        var userId = HttpContext.Session.GetString("user_first_name");
        var username = HttpContext.Session.GetString("user_last_name");
        var email = HttpContext.Session.GetString("user_email");
<<<<<<< Updated upstream
        Console.WriteLine(userId + "  "+username+ "  " + email);
=======
        Console.WriteLine(userId + "  " + username + "  " + email + "  " + userI);
>>>>>>> Stashed changes
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    public IActionResult Doctors()
    {
<<<<<<< Updated upstream
        
        return RedirectToAction("Calendar","Calendar");
=======
        return RedirectToAction("HomeDoctor", "Doctor");
>>>>>>> Stashed changes
    }

    public IActionResult UI_Patient()
    {
        return View();
    }

    public IActionResult UI_doctor()
    {
        return View();
    }

    public IActionResult UI_admin()
    {
        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.SetString("IsLoggedIn", "true");
        HttpContext.Session.Clear();
        ViewBag.P = null;
        return RedirectToAction("Index");
    }

<<<<<<< Updated upstream

    public void push_patient(string email)
=======
    public void push_patient(int id, string email, string role)
>>>>>>> Stashed changes
    {
        HttpContext.Session.SetString("user_first_name", email);
        HttpContext.Session.SetString("user_last_name", email);
        HttpContext.Session.SetString("user_email", email);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitForm(string firstName, string lastName, string email, string password)
    {
        using (var connection = ModifUser.ConnectToDatabase())
        {
<<<<<<< Updated upstream
                ModifUser.InsertUser(connection, firstName,lastName,email,password);
=======
            //ModifUser.InsertUser(connection, firstName,lastName,email,password);
            int maxId = 0;
            const string max_id = "SELECT MAX(user_id) AS max_id FROM users";
            using (var command = new SqliteCommand(max_id, connection))
            {
                object result = command.ExecuteScalar();
                maxId = result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }

            _context.Users.Add(new User
            {
                user_first_name = firstName,
                user_last_name = lastName,
                user_email = email,
                user_password = password,
                user_id = maxId + 1,
                user_role = "P"
            });

            _context.SaveChanges();
            _context.Patient.Add(new Patients
            {
                patient_email = email,
                patient_id = maxId + 1,
                patient_last_name = lastName,
                patient_name = firstName
            });

            _context.SaveChanges();
            AssignRole(userId: maxId + 1.ToString(), roleName: "P").Wait();
            var user = await _userManager.FindByIdAsync(maxId + 1.ToString() );
            if (user != null)
            {
                bool hasRole = await _userManager.IsInRoleAsync(user, "P");
                if (hasRole)
                {
                    Console.WriteLine("L'utilisateur a bien le rôle Admin.");
                }
                else
                {
                    Console.WriteLine("L'utilisateur n'a pas le rôle Admin.");
                }
            }
            return RedirectToAction("Index","Home");
            
            
>>>>>>> Stashed changes
        }
    }

    public IActionResult SubmitLogin(string email, string password)
    {
        
        using (var connection = ModifUser.ConnectToDatabase())
        {
            int isAuthenticated = ModifUser.is_user(connection, email, password);
<<<<<<< Updated upstream
        
            if (isAuthenticated == 1)
            {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                push_patient(email);
                return RedirectToAction("HomeDoctor","Doctor");
=======

            if (isAuthenticated == 1)
            {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                int id = ModifUser.get_id(connection, email);
                string role = ModifUser.get_role(connection, email);
                push_patient(id, email, role);
                HttpContext.Session.SetInt32("doctor_id", id);
                return RedirectToAction("HomeDoctor", "Doctor");
>>>>>>> Stashed changes
            }
            if (isAuthenticated == 2)
            {
                HttpContext.Session.SetString("IsLoggedIn", "true");
<<<<<<< Updated upstream
                push_patient(email);
=======
                int id = ModifUser.get_id(connection, email);
                string role = ModifUser.get_role(connection, email);
                push_patient(id, email, role);
>>>>>>> Stashed changes
                return RedirectToAction("UI_patient");
            }
            if (isAuthenticated == 3)
            {
                HttpContext.Session.SetString("IsLoggedIn", "true");
<<<<<<< Updated upstream
                push_patient(email);
                return RedirectToAction("UI_admin");
            }
            return View("Index"); 
=======
                int id = ModifUser.get_id(connection, email);
                push_patient(id, email, "A");
                return RedirectToAction("UI_admin");
            }
            ViewBag.incorrect = true;
            return View("Login");
>>>>>>> Stashed changes
        }
    }

    public void SubmitDoctors(string firstName, string lastName, string email, string password)
    {
        using (var connection = ModifUser.ConnectToDatabase())
        {
<<<<<<< Updated upstream
            modif_doctors.InsertDoctors(connection, firstName,lastName,email,password);
               
=======
            //modif_doctors.InsertDoctors(connection, firstName,lastName,email,password);
>>>>>>> Stashed changes
        }
    }

    [HttpPost]
    public async Task<IActionResult> AssignRole(string userId, string roleName)
    {
        
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            await _roleManager.CreateAsync(new IdentityRole(roleName));
        }

       
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound(new { message = "Utilisateur non trouvé" });
        }

        
        var result = await _userManager.AddToRoleAsync(user, roleName);
        if (result.Succeeded)
        {
            return Ok(new { message = "Rôle assigné avec succès" });
        }

        //return RedirectToAction("HomeDoctor", "Doctor");
        return BadRequest(new { message = "Erreur lors de l'assignation du rôle", errors = result.Errors });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}