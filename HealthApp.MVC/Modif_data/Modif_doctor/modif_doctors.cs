using Microsoft.Data.Sqlite;
using System;

namespace SQLiteConnectionTest
{
    public static class modif_doctors
    {
        static string connectionString = "Data Source=HealthApp.MVC.db";

        // Fonction pour se connecter à la base de données
        public static SqliteConnection ConnectToDatabase()
        {
            try
            {
                var connection = new SqliteConnection(connectionString);
                connection.Open();
                Console.WriteLine("Connexion à la base de données réussie !");
                return connection;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur de connexion : {ex.Message}");
                return null;
            }
        }

        
        

        
        public static void InsertDoctors(SqliteConnection connection, string first_name ,string last_name, string email,string password)
        {
            
            int maxId = 0;
            string max_id ="SELECT MAX(user_id) AS max_id FROM users";
            using (var command = new SqliteCommand(max_id, connection))
            {
                
 
                object result = command.ExecuteScalar();

                // Vérifier si le résultat est nul (si la table est vide, cela peut renvoyer null)
                maxId = result != DBNull.Value ? Convert.ToInt32(result) : 0;
                Console.WriteLine(maxId);
            }
            string query = "INSERT INTO users (user_id, user_first_name,user_last_name , user_email,user_password) VALUES (@user_id, @user_first_name, @user_last_name, @user_email, @user_password);";
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@user_id", maxId+1);
                command.Parameters.AddWithValue("@user_first_name", first_name);
                command.Parameters.AddWithValue("@user_last_name", last_name);
                command.Parameters.AddWithValue("@user_email", email);
                command.Parameters.AddWithValue("@user_password", password);

                command.ExecuteNonQuery();
                Console.WriteLine("Utilisateur inséré avec succès.");
            }
            
            string query_doctor = "INSERT INTO doctors (doctor_id, doctor_first_name, doctor_last_name,doctor_email) VALUES (@doctor_id, @doctor_first_name, @doctor_email, @doctor_last_name);";
            using (var command2 = new SqliteCommand(query_doctor, connection))
            {
                command2.Parameters.AddWithValue("@doctor_id", maxId+1);
                command2.Parameters.AddWithValue("@doctor_first_name", first_name);
                command2.Parameters.AddWithValue("@doctor_last_name", last_name);
                command2.Parameters.AddWithValue("@doctor_email", email);
                

                command2.ExecuteNonQuery();
                
            }
            
            
        }


        
        
        
        
        
        
        
        
        
        
        
        

        
    }
}
