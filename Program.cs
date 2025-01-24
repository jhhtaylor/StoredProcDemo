using System;
using Microsoft.Data.SqlClient;

class Program
{
    static void Main()
    {
        var connectionString = "Server=localhost,1433;Database=UserDB;User Id=sa;Password=Your@StrongPass123!;Integrated Security=False;TrustServerCertificate=True;";

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                
                using (SqlCommand command = new SqlCommand("InsertUser", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    
                    // Input parameters
                    command.Parameters.AddWithValue("@UserName", "JohnDoe");
                    command.Parameters.AddWithValue("@Email", "john@example.com");
                    
                    // Output parameter
                    SqlParameter outputParam = new SqlParameter("@NewUserID", System.Data.SqlDbType.Int);
                    outputParam.Direction = System.Data.ParameterDirection.Output;
                    command.Parameters.Add(outputParam);
                    
                    command.ExecuteNonQuery();
                    
                    Console.WriteLine($"New User ID: {outputParam.Value}");
                }
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"SQL Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}