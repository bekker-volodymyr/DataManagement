using Microsoft.Data.Sqlite;
using Dapper;
using DapperIntro.Entities;

namespace DapperIntro;

public class Program
{
    public static void Main(string[] args)
    {
        // Створюємо рядок підключення до бази даних SQLite
        string connectionString = "Data Source=library.db;";

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        CreateDatabaseAndTable(connection);

        var books = ReadAllBooks(connection);
        foreach (var book in books)
        {
            Console.WriteLine($"Id: {book.Id}, Title: {book.Title}, Author: {book.Author}");
        }

        // Читання скалярного значення
        var bookCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Books;");
        Console.WriteLine($"Total number of books: {bookCount}");

        // Отримання одного запису
        var firstBook = connection.QueryFirstOrDefault<Book>(@"
            SELECT * FROM Books WHERE Id = @Id;", new { Id = 1 });

        // Мультизапит
        var multiQuery = connection.QueryMultiple(@"
            SELECT * FROM Books;
            SELECT COUNT(*) FROM Books;");

        var allBooks = multiQuery.Read<Book>().ToList();
        var totalBooks = multiQuery.ReadSingle<int>();

        Console.WriteLine($"Total books from multi-query: {totalBooks}");

    }

    public static void CreateDatabaseAndTable(SqliteConnection connection)
    {
        // SQL запит для створення таблиці Books
        string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS Books (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Author TEXT NOT NULL
            );";

        // Виконуємо запит на створення таблиці
        connection.Execute(createTableQuery);
    }

    public static void AddBook(SqliteConnection connection, Book book)
    {
        // Параметризований запит для вставки книги
        string insertOneBook = @"
            INSERT INTO Books (Title, Author) 
            VALUES (@Title, @Author);";

        // Виконуємо запит на вставку книги
        connection.Query(insertOneBook, book);
    }

    public static List<Book> ReadAllBooks(SqliteConnection connection)
    {
        // SQL запит для читання всіх книг
        string selectAllBooks = "SELECT * FROM Books;";

        // Виконуємо запит і повертаємо список книг
        return connection.Query<Book>(selectAllBooks).ToList();
    }

    public static void UpdateBook(SqliteConnection connection, string title)
    {
        // Параметризований запит для оновлення книги
        string updateBookQuery = @"
            UPDATE Books 
            SET Author = 'Frank Herbert' 
            WHERE Title = @Title;";

        // Виконуємо запит на оновлення книги
        connection.Execute(updateBookQuery, new { Title = title });
    }

    public static void DeleteBookById(SqliteConnection connection, int id)
    {
        // Параметризований запит для видалення книги
        string deleteBookQuery = "DELETE FROM Books WHERE Id = @Id;";

        // Виконуємо запит на видалення книги
        connection.Execute(deleteBookQuery, new { Id = id });
    }
}
