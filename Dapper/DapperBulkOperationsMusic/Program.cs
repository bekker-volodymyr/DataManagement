using Dapper;
using DapperBulkOperationsMusic.Entities;
using Z.Dapper.Plus;
using MySqlConnector;

public class Program
{
    private readonly static string _masterConnectionString = "Server=localhost;User=appuser;Password=strongpassword;";
    private readonly static string _connectionString = "Server=localhost;Database=music;User=appuser;Password=strongpassword;";

    public static void Main(string[] args)
    {
        // Реєстрація моделей для Dapper Plus
        DapperPlusManager.Entity<Artist>().Table("Artists").Key(a => a.Id);
        DapperPlusManager.Entity<Song>().Table("Songs").Key(s => s.Id);

        InitMySql();
        using var db = new MySqlConnection(_connectionString);
        db.Open();
        CreateDatabase(db);

        // Insert
        var artists = new List<Artist>
        {
            new Artist { Name = "Жадан і Собаки" },
            new Artist { Name = "jockii druce" },
            new Artist { Name = "Sabrina Carpenter" },
            new Artist { Name = "Kendrik" },
            new Artist { Name = "Tyler Creator" }
        };
        db.BulkInsert(artists);

        // Update
        artists[3].Name = "Kendrik Lamar";
        artists[4].Name = "Tyler, the Creator";
        db.BulkUpdate(artists);

        // Delete
        db.BulkDelete(artists);

        // Merge (Insert або Update по Id)
        artists[0].Name = "Жадан і Собаки (оновлено)"; // Оновить існуючого
        artists[1].Name = "jockii druce (оновлено)";
        artists.Add(new Artist { Name = "OTOY" }); // Додасть нового
        db.BulkMerge(artists);


        // Bulk Insert Songs
        var songs = new List<Song>
        {
            new Song { ArtistId = artists[0].Id, Title = "Автозак" },
            new Song { ArtistId = artists[0].Id, Title = "Мадонна" },
            new Song { ArtistId = artists[0].Id, Title = "Вафлі артек" },
            new Song { ArtistId = artists[1].Id, Title = "іноді" },
            new Song { ArtistId = artists[2].Id, Title = "Espresso" },
            new Song { ArtistId = artists[3].Id, Title = "United in Grief" },
            new Song { ArtistId = artists[3].Id, Title = "Not Like Us" },
            new Song { ArtistId = artists[4].Id, Title = "CHROMAKOPIA" },
        };

        db.BulkMerge(songs);
    }

    static void InitMySql()
    {
        // Підключення до MySQL сервера
        using var conn = new MySqlConnection(_masterConnectionString);
        conn.Open();

        // Створення бази даних, якщо вона не існує
        conn.Execute("CREATE DATABASE IF NOT EXISTS music;");

        Console.WriteLine("Базу створено й підключено");
    }

    static void CreateDatabase(MySqlConnection dbConn)
    {
        // Створення таблиць, якщо вони не існують
        dbConn.Execute(@"
            CREATE TABLE IF NOT EXISTS Artists (
                Id CHAR(36) PRIMARY KEY,
                Name VARCHAR(255) NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Songs (
                Id CHAR(36) PRIMARY KEY,
                ArtistId CHAR(36) NOT NULL,
                Title VARCHAR(255) NOT NULL,
                FOREIGN KEY (ArtistId) REFERENCES Artists(Id)
            );
        ");

        Console.WriteLine("Таблиці створено");
    }
}