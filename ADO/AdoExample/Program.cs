using Microsoft.Data.SqlClient;
using System.Text;

namespace AdoExample
{
    public class Program
    {
        const string ConnectionString =
            @"Data Source=BEKKER\ITSTEP_SQLSERV;
                Integrated Security=True;
                Persist Security Info=False;Pooling=False;
                MultipleActiveResultSets=False;Encrypt=True;
                TrustServerCertificate=True;Command Timeout=0";

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;

            SetupDatabase();

            Console.WriteLine("Введіть ім'я користувача для пошуку: ");
            string search = Console.ReadLine();
            UnsafeSearch(search);

            SafeSearch(search);
        }

        private static void UnsafeSearch(string name)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string sql = "SELECT Id, Username, Salary FROM Users WHERE Username = '" + name + "'";

                Console.WriteLine($"[DEBUG] Виконуємо SQL: {sql}");

                using (var command = new SqlCommand(sql, connection))
                {
                    try
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                Console.WriteLine("РЕЗУЛЬТАТ ПОШУКУ:");
                                while (reader.Read())
                                    Console.WriteLine($"Користувач: {reader["Username"]} | ЗП: {reader["Salary"]}");
                            }
                            else
                                Console.WriteLine("Нікого не знайдено.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
        }

        static void SafeSearch(string inputName)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                string sql = "SELECT Id, Username, Salary FROM Users WHERE Username = @nameParam";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@nameParam", inputName));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                decimal salary = reader.GetDecimal(2);

                                Console.WriteLine($"User: {id} | {name} | Salary: {salary}$");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Користувача не знайдено.");
                        }
                    }
                }
            }
        }

        private static void SetupDatabase()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var command = @"
                    IF OBJECT_ID('Users', 'U') IS NULL
                    BEGIN
                        CREATE TABLE Users  (Id INT IDENTITY PRIMARY KEY, Username NVARCHAR(50), Salary DECIMAL(18,2));
                        INSERT INTO Users VALUES ('Admin', 9999.00), ('Student', 500.00), ('Teacher', 2000.00);
                    END
                ";

                using (var sqlCommand = new SqlCommand(command, connection))
                {
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
