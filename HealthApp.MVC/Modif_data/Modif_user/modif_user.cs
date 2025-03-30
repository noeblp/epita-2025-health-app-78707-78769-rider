using Microsoft.Data.Sqlite;
using HealthApp.Domain;

namespace hospital.Modif_data
{
    public static class ModifUser 
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
        
        

        
        

        // Fonction pour insérer un utilisateur dans la table 'users'
        public static void InsertUser(SqliteConnection connection, string firstName ,string lastName, string email,string password)
        {
            int maxId = 0;
            const string max_id = "SELECT MAX(user_id) AS max_id FROM users";
            using (var command = new SqliteCommand(max_id, connection))
            {
                object result = command.ExecuteScalar();
                maxId = result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }
            
            
            string query = "INSERT INTO users (user_id, user_first_name,user_last_name , user_email,user_password,user_role) VALUES (@user_id, @user_first_name, @user_last_name, @user_email, @user_password,@user_role);";
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@user_id", maxId+1);
                command.Parameters.AddWithValue("@user_first_name", firstName);
                command.Parameters.AddWithValue("@user_last_name", lastName);
                command.Parameters.AddWithValue("@user_email", email);
                command.Parameters.AddWithValue("@user_password", password);
                command.Parameters.AddWithValue("@user_role", "P");
                
                command.ExecuteNonQuery();
                Console.WriteLine("Utilisateur inséré avec succès.");
            }
            
            string queryPatient = "INSERT INTO patient (patient_id, patient_name, patient_last_name,patient_email) VALUES (@patient_id, @patient_name, @patient_last_name, @patient_email);";
            using (var command2 = new SqliteCommand(queryPatient, connection))
            {
                command2.Parameters.AddWithValue("@patient_id", maxId+1);
                command2.Parameters.AddWithValue("@patient_name", firstName);
                command2.Parameters.AddWithValue("@patient_last_name", lastName);
                command2.Parameters.AddWithValue("@patient_email", email);
                

                command2.ExecuteNonQuery();
                
            }
            
            
        }


        public static int is_user(SqliteConnection connection, string username, string password)
        {
            
            var query = "SELECT user_role FROM users WHERE user_email = @user_email AND user_password = @user_password";

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
                return 0; // Autres rôles
            }
            return 0; // Aucun utilisateur trouvé
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
<<<<<<< Updated upstream
=======

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

        public static void UpdateUserRole(SqliteConnection connection, int userId, string newRole)
        {
            string query = "UPDATE users SET user_role = @newRole WHERE user_id = @userId";
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@newRole", newRole);
                command.Parameters.AddWithValue("@userId", userId);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
>>>>>>> Stashed changes
    }
}
