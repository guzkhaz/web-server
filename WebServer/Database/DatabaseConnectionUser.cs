using System.Data;
using Npgsql;
using WebServer.Models;

namespace WebServer.DataBase;

public class DatabaseConnectionUser
{
    private readonly NpgsqlConnection _connection = new(ConnectionString);
    private const string ConnectionString = "Host=localhost:5432;" +
                                            "Username=postgres;" +
                                            "Password=12345;" +
                                            "Database=SemWork";
    
     public async Task AddUserAsync(User user, CancellationToken ctx = default)
    {
        await _connection.OpenAsync(ctx);
        try
        {
            await using (var cmd = new NpgsqlCommand("add_users", _connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                
                cmd.Parameters.AddWithValue("_name", user.Name);
                cmd.Parameters.AddWithValue("_surname", user.Surname);
                cmd.Parameters.AddWithValue("_email", user.Email);
                cmd.Parameters.AddWithValue("_password", user.Password);

                await cmd.ExecuteNonQueryAsync(ctx);
            }
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }
     
     public async Task<bool> UserExistAsync(User? user, CancellationToken ctx = default)
     {
         await _connection.OpenAsync(ctx);
         try
         {
             var commandText = $@"SELECT * from users WHERE email=@email";
             await using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, _connection))
             {
                 cmd.Parameters.AddWithValue("email", user.Email);
                 var reader = cmd.ExecuteReader();
                 if (reader.HasRows)
                 {
                     return true;
                 }
             }
             return false;
         }
         finally
         {
             await _connection.CloseAsync();
         }
     }
     
     public async Task EditUserAsync(User? user, CancellationToken ctx = default)
     {
         await _connection.OpenAsync(ctx);
         try
         {
             await using (var cmd = new NpgsqlCommand("edit_user", _connection))
             {
                 cmd.CommandType = CommandType.StoredProcedure;
                
                 cmd.Parameters.AddWithValue("_email", user.Email);
                 cmd.Parameters.AddWithValue("_name", user.Name);
                 cmd.Parameters.AddWithValue("_surname", user.Surname);
                 cmd.Parameters.AddWithValue("_image_path", user.Image ?? "");
                 cmd.Parameters.AddWithValue("_location", user.Location ?? "");
                 await cmd.ExecuteNonQueryAsync(ctx);
             }
         }
         finally
         {
             await _connection.CloseAsync();
         }
     }
     
     public async Task<User?> GetUserAsync(string email, CancellationToken ctx = default)
     {
         await _connection.OpenAsync(ctx);
         try
         {
             string commandText = $@"SELECT * FROM users where email='{email}'";
            
             await using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, _connection))
             {
                 await using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync(ctx))
                     while (await reader.ReadAsync(ctx))
                     {
                         var user = ReadUser(reader);
                         return user;
                     }
             }
             return null;
         }
         finally
         {
             await _connection.CloseAsync();
         }
     }
     private static User ReadUser(NpgsqlDataReader reader)
     {
         var name = reader["name"] as string;
         var surname = reader["surname"] as string;
         var email = reader["email"] as string;
         var password = reader["password"] as string;
         var location = reader["location"] as string;
         var image = reader["image_path"] as string;

         var user = new User()
         {
             Name = name,
             Surname = surname,
             Email = email,
             Password = password,
             Location = location,
             Image = image
         };
         return user;
     }
     
     public async Task SupplementUserAsync(User? user, CancellationToken ctx = default)
     {
         await _connection.OpenAsync(ctx);
         try
         {
             await using (var cmd = new NpgsqlCommand("supplement_user", _connection))
             {
                 cmd.CommandType = CommandType.StoredProcedure;
                
                 cmd.Parameters.AddWithValue("_email", user.Email);
                 cmd.Parameters.AddWithValue("_image_path", user.Image ?? "");
                 cmd.Parameters.AddWithValue("_location", user.Location ?? "");

                 await cmd.ExecuteNonQueryAsync(ctx);
             }
         }
         finally
         {
             await _connection.CloseAsync();
         }
     }
     
     public async Task DeleteUserAsync(string email, CancellationToken ctx = default)
     {
         await _connection.OpenAsync(ctx);
         try
         {
             await using (var cmd = new NpgsqlCommand("delete_user", _connection))
             {
                 cmd.CommandType = CommandType.StoredProcedure;
                
                 cmd.Parameters.AddWithValue("_email", email);

                 await cmd.ExecuteNonQueryAsync(ctx);
             }
         }
         finally
         {
             await _connection.CloseAsync();
         }
     }
}