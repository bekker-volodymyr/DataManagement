using Dapper;
using DapperRelations.Entities;
using Microsoft.Data.Sqlite;

namespace DapperRelations;

public class Program
{
    public static void Main(string[] args)
    {
        using var connection = new SqliteConnection("Data Source=library.db");
        connection.Open();

        CreateDatabase(connection);
        //AddVisitor(connection);
        //ReadAllVisitors(connection);
        //AddAuthorWithBooks(connection);
        //ReadAuthorsWithBooks(connection);
        //AddLoan(connection);
        //ReadLoans(connection);
    }

    private static void ReadLoans(SqliteConnection connection)
    {
        string readLoans = @"
            SELECT 
                b.Id as BookId, b.Title,
                v.Id as VisitorId, v.Name,
                l.LoanDate, l.ReturnDate
            FROM Books b
            JOIN BookLoans l ON b.Id = l.BookId
            JOIN Visitors v ON v.Id = l.VisitorId
        ";

        var visitorMap = new Dictionary<long, Visitor>();

        var visitors = connection.Query<Book, Visitor, string, string, Visitor>(
            readLoans,
            (book, visitor, loanDate, returnDate) =>
            {
                if (!visitorMap.TryGetValue(visitor.Id, out var v))
                {
                    v = visitor;
                    v.Loans = new List<LoanInfo>();
                    visitorMap[visitor.Id] = v;
                }

                v.Loans.Add(new LoanInfo
                {
                    Book = book,
                    LoanDate = DateTime.Parse(loanDate),
                    ReturnDate = string.IsNullOrEmpty(returnDate) ? null : DateTime.Parse(returnDate)
                });

                return v;
            },
            splitOn: "VisitorId,LoanDate,ReturnDate"
        ).Distinct().ToList();

        foreach (var v in visitors)
        {

            Console.WriteLine($"{v.Name} loans: ");
            if (v.Loans.Count == 0)
            {
                Console.WriteLine("\tNo loans");
            }
            else
            {
                foreach (var l in v.Loans)
                {
                    Console.WriteLine($"\t{l.Book.Title} -- {l.LoanDate} -- {(l.ReturnDate is null ? "Waiting to return" : l.ReturnDate)}");
                }
            }
        }
    }

    private static void AddLoan(SqliteConnection connection)
    {
        var book = connection.QueryFirstOrDefault("SELECT * FROM Books LIMIT 1;");
        var visitor = connection.QueryFirstOrDefault("SELECT * FROM Visitors LIMIT 1;");

        connection.Execute(
            "INSERT INTO BookLoans (BookId, VisitorId, LoanDate, ReturnDate) VALUES (@BookId, @VisitorId, @LoanDate, @ReturnDate);",
            new
            {
                BookId = book.Id,
                VisitorId = visitor.Id,
                LoanDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                ReturnDate = (DateTime?)null
            });
    }

    private static void ReadAuthorsWithBooks(SqliteConnection connection)
    {
        string readBooks = @"
            SELECT a.Id, a.Name,
                b.Id, b.Title, b.AuthorId
            FROM Authors a
            LEFT JOIN Books b ON a.Id = b.AuthorId
        ";

        var authorMap = new Dictionary<long, Author>();

        var authors = connection.Query<Author, Book, Author>(
            readBooks,
            (a, b) =>
            {
                if (!authorMap.TryGetValue(a.Id, out var author))
                {
                    author = a;
                    author.Books = new List<Book>();
                    authorMap.Add(author.Id, author);
                }

                if (b != null)
                    author.Books.Add(b);

                return author;
            },
            splitOn: "Id"
        ).Distinct().ToList();

        foreach (var a in authors)
        {
            Console.WriteLine(a.Name);
            foreach (var b in a.Books)
            {
                Console.WriteLine($"\t{b.Title}");
            }
        }
    }

    private static void AddAuthorWithBooks(SqliteConnection connection)
    {
        var transaction = connection.BeginTransaction();

        string insertAuthor = @"
            INSERT INTO Authors ([Name], [DateOfBirth])
            VALUES (@Name, @DateOfBirth);
            SELECT last_insert_rowid();";
        string insertBook = @"
            INSERT INTO Books ([Title], [AuthorId])
            VALUES (@Title, @AuthorId)";

        var authorId = connection.ExecuteScalar<long>(
            insertAuthor,
            new { Name = "Ліна Костенко", DateOfBirth = new DateTime(1930, 3, 19) },
            transaction
        );

        connection.Execute(
            insertBook,
            new[] {
                new { Title = "Маруся Чурай", AuthorId = authorId },
                new { Title = "Записки українського самашедшого", AuthorId = authorId }
            },
            transaction
        );

        transaction.Commit();
    }

    private static void ReadAllVisitors(SqliteConnection connection)
    {
        // Читання
        string readVisitorsWithPassports = @"
            SELECT v.Id, v.Name, v.PhoneNumber, v.DateOfBirth,
                p.PassportNumber, p.VisitorId
            FROM Visitors v
            JOIN Passports p ON v.Id = p.VisitorId";


        // Visitor — перший тип, який мапиться з колонок до splitOn.
        // Passport — другий тип, мапиться з колонок після splitOn.
        // останній Visitor — тип, який повертає лямбда і який буде у результаті.
        var visitors = connection.Query<Visitor, Passport, Visitor>(
            readVisitorsWithPassports,
            // Ініціалізація навігаційної властивості
            (v, p) =>
            {
                v.Passport = p;
                p.Visitor = v;
                return v;
            },
            splitOn: "PassportNumber"
        ).ToList();

        foreach (var v in visitors)
        {
            Console.WriteLine(v);
        }
    }

    private static void AddVisitor(SqliteConnection connection)
    {
        // 1. Створити транзакцію
        // 2. Додати відвідувача
        // 3. Отримати Id нового відвідувача 
        // 4. Додати паспорт разом з Id відвідувача

        using var transaction = connection.BeginTransaction();

        // Запит для SQLServer
        // string insertVisitor = @"INSERT INTO Visitors ([Name], [PhoneNumber], [DateOfBirth]) 
        //                          OUTPUT INSERTED.Id VALUES (@Name, @PhoneNumber, @DateOfBirth)";
        string insertVisitor = @"INSERT INTO Visitors ([Name], [PhoneNumber], [DateOfBirth]) 
                                 VALUES (@Name, @PhoneNumber, @DateOfBirth);
                                 SELECT last_insert_rowid();";
        string insertPassport = @"INSERT INTO Passports (PassportNumber, VisitorId) VALUES (@PassportNumber, @VisitorId)";

        long visitorId = connection.ExecuteScalar<long>(insertVisitor, new
        {
            Name = "Михайло Грушевський",
            PhoneNumber = "+380508579453",
            DateOfBirth = new DateTime(1866, 9, 29)
        }, transaction);

        connection.Execute(insertPassport, new { PassportNumber = "019283746", VisitorId = visitorId }, transaction);

        transaction.Commit();
    }

    private static void CreateDatabase(SqliteConnection connection)
    {
        var createTableQuery = @"
        CREATE TABLE IF NOT EXISTS Authors (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
            DateOfBirth TEXT NOT NULL
        );

        CREATE TABLE IF NOT EXISTS Books (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Title TEXT NOT NULL,
            AuthorId INTEGER NOT NULL,
            FOREIGN KEY (AuthorId) REFERENCES Authors(Id)
        );

        CREATE TABLE IF NOT EXISTS Visitors (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
            PhoneNumber TEXT NOT NULL,
            DateOfBirth TEXT NOT NULL
        );

        CREATE TABLE IF NOT EXISTS Passports (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            PassportNumber TEXT NOT NULL,
            VisitorId INTEGER NOT NULL UNIQUE,
            FOREIGN KEY (VisitorId) REFERENCES Visitors(Id)
        );

        CREATE TABLE IF NOT EXISTS BookLoans (
            BookId INTEGER NOT NULL,
            VisitorId INTEGER NOT NULL,
            LoanDate TEXT NOT NULL,
            ReturnDate TEXT,
            PRIMARY KEY (BookId, VisitorId, LoanDate),
            FOREIGN KEY (BookId) REFERENCES Books(Id),
            FOREIGN KEY (VisitorId) REFERENCES Visitors(Id)
        );
        ";

        connection.Execute(createTableQuery);
    }

}