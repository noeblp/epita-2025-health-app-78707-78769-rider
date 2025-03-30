using System;
using Microsoft.Data.Sqlite;
using HealthApp.Domain;
using Microsoft.AspNetCore.Http;

namespace hospital.Modif_data
{
    public class ModifUser 
    {
        private const string ConnectionString = "Data Source=hospital.db";


        public static SqliteConnection ConnectToDatabase()
        {
            try
            {
                var connection = new SqliteConnection(ConnectionString);
                connection.Open();
                
                Console.WriteLine("Connexion à la base de données réussie !");
                return connection;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur de connexion : {ex.Message}");
                return null!;
            }
        }
        
        

        
        

        
        


        public static int is_user(SqliteConnection connection, string username, string password)
        {
            
            var query = "SELECT user_role, user_id FROM users WHERE user_email = @user_email AND user_password = @user_password";

            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@user_email", username);
            command.Parameters.AddWithValue("@user_password", password);
            

            connection.Open();
            var result = command.ExecuteScalar();
            connection.Close();

            if (result != null) 
            {
                string? userRole = result.ToString(); 
                
                if (userRole == "D") 
                {
                    return 1;
                }
                if (userRole == "P") 
                {
                    return 2;
                }
                return 3; // admin
            }
            return 0; 
        }


        public static int get_id(SqliteConnection connection, string email)
        {
            var query = "SELECT user_id FROM users WHERE user_email = @email";
            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@email", email);
            connection.Open();
            var result = command.ExecuteScalar();
            connection.Close();
            return result != null ? Convert.ToInt32(result) : -1;
        }
        
        public static string get_role(SqliteConnection connection, string email)
        {
            var query = "SELECT user_role FROM users WHERE user_email = @email";
            using SqliteCommand command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@email", email);
            connection.Open();
            var result = command.ExecuteScalar();
            connection.Close();
            return result.ToString() ;
        }
        
        
        public static void DeleteAll(SqliteConnection connection)
        {
            string query = "DELETE FROM users;";
            using (var command = new SqliteCommand(query, connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine("tout sup");
            }
        }

        public static void update_user(SqliteConnection connection, string name, string lastname, string email,
            string password)
        {
            string query = "UPDATE users SET user_first_name = @name, user_last_name = @lastname, user_password = @password WHERE user_email = @email";
            Console.WriteLine("updateuser called!");
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@lastname", lastname);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@email", email); 

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

            
        }
    }
}
