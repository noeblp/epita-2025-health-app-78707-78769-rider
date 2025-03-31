using HealthApp.Razor.Data;
using hospital.Modif_data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;


namespace hospital.Controllers;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly string _connectionString = "Data Source=hospital.db";


    public AdminController(ApplicationDbContext context)
    {

        _context = context;
        _context.Database.EnsureCreated();


    }



    public IActionResult AddDoctor()
    {
        return View();
    }

    public IActionResult Manage()
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
        List<Dictionary<string, object>> users = new List<Dictionary<string, object>>();

        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();
            var command =
                new SqliteCommand("SELECT user_id, user_first_name, user_last_name, user_email, user_role FROM users",
                    connection);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var user = new Dictionary<string, object>
                {
                    ["user_id"] = reader["user_id"],
                    ["user_first_name"] = reader["user_first_name"],
                    ["user_last_name"] = reader["user_last_name"],
                    ["user_email"] = reader["user_email"],
                    ["user_role"] = reader["user_role"]
                };
                users.Add(user);
            }
        }

        ViewBag.Users = users;
        return View("UserList");
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

        return RedirectToAction("UserList");
    }

    [HttpGet]
    public IActionResult EditUser(int id)
    {
        using (var connection = ModifUser.ConnectToDatabase())
        {
            connection.Open();
            using (var command =
                   new SqliteCommand(
                       "SELECT user_id, user_first_name, user_last_name, user_email, user_role FROM users WHERE user_id = @id",
                       connection))
            {
                command.Parameters.AddWithValue("@id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var user = new Dictionary<string, object>
                        {
                            ["user_id"] = reader["user_id"],
                            ["user_first_name"] = reader["user_first_name"],
                            ["user_last_name"] = reader["user_last_name"],
                            ["user_email"] = reader["user_email"],
                            ["user_role"] = reader["user_role"]
                        };

                        ViewBag.User = user;
                        return View("EditUser");
                    }
                }
            }

            connection.Close();
        }

        // If no user is found, redirect back to user list with an error message
        TempData["Error"] = "User not found!";
        return RedirectToAction("UserList");
    }

    [HttpGet]
    public IActionResult AddUser()
    {
        return View();
    }
    

        [HttpPost]
        public IActionResult AddUser(string user_first_name, string user_last_name, string user_email,
            string user_password, string user_role)
        {
            using (var connection = ModifUser.ConnectToDatabase())
            {
                var query =
                    "INSERT INTO users (user_first_name, user_last_name, user_email, user_password, user_role) VALUES (@firstName, @lastName, @email, @password, @role)";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@firstName", user_first_name);
                    command.Parameters.AddWithValue("@lastName", user_last_name);
                    command.Parameters.AddWithValue("@email", user_email);
                    command.Parameters.AddWithValue("@password", user_password); // Ideally, hash this password
                    command.Parameters.AddWithValue("@role", user_role);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            //####################################CHANGE REDIRECTION###########################################
            return RedirectToAction("UserList"); // Redirect back to the user list
    }
}


