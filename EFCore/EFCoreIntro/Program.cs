using EFCoreIntro.Entities;

namespace EFCoreIntro
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Ініціалізація контексту
            AppDbContext db = new AppDbContext();

            // Створення об'єктів сутностей
            Student student = new Student() { FirstName = "Антон", LastName = "Степанюк", Age = 17 };

            // Додавання до БД
            db.Students.Add(student);

            // Збереження доданих записів
            db.SaveChanges();

            // Читання даних
            var students = db.Students.ToList();
            foreach (var s in students)
            {
                Console.WriteLine($"Id: {s.Id}\nІм'я: {s.LastName} {s.FirstName}\nВік: {s.Age}\nScholarship: {(s.Scholarship is null ? 0 : s.Scholarship)}");
            }

            Console.Write("Введіть Id студента, щоб його видалити: ");
            int id = Int32.Parse(Console.ReadLine());

            // Пошук студента за Id
            Student? stToRemove = db.Students.FirstOrDefault(s => s.Id == id);
            // Видалення, якщо знайдено
            if (stToRemove != null)
            {
                db.Students.Remove(stToRemove);
                db.SaveChanges();
                Console.WriteLine("Студента видалено");
            }
            else
            {
                Console.WriteLine($"Немає студента з Id {id}");
            }

            Console.WriteLine("Введіть Id студента, якого хочете оновити: ");
            id = Int32.Parse(Console.ReadLine());

            // Пошук студента для оновлення
            Student? stToUpdate = db.Students.FirstOrDefault(s => s.Id == id);
            // Введення нових даних
            if (stToUpdate != null)
            {
                Console.WriteLine("Введіть нове ім'я: ");
                stToUpdate.FirstName = Console.ReadLine();
                Console.WriteLine("Введіть нове прізвище: ");
                stToUpdate.LastName = Console.ReadLine();
                Console.WriteLine("Введіть новий вік: ");
                stToUpdate.Age = Int32.Parse(Console.ReadLine());
                Console.WriteLine("Введіть нову стипендію: ");
                stToUpdate.Scholarship = Single.Parse(Console.ReadLine().Replace('.', ','));
                //Збереження змін
                db.SaveChanges();
            }
            else
            {
                Console.WriteLine($"Немає студента з Id {id}");
            }

            PrintStudents(db);
        }

        static void PrintStudents(AppDbContext db)
        {
            var students = db.Students.ToList();
            foreach (var s in students)
            {
                Console.WriteLine($"Id: {s.Id}\nІм'я: {s.LastName} {s.FirstName}\nВік: {s.Age}\nScholarship: {(s.Scholarship is null ? 0 : s.Scholarship)}");
            }
        }
    }
}
