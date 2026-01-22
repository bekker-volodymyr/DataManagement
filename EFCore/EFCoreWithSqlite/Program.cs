namespace EFCoreWithSqlite
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Student student = new Student()
            {
                Age = 17,
                FirstName = "Степан",
                LastName = "Гіга"
            };

            AppDbContext db = new AppDbContext();

            Console.WriteLine($"Додати студента {student.LastName} {student.FirstName} до БД? (Y/N): ");
            string? answer = Console.ReadLine();

            if (answer?.ToLower() == "y")
            {
                db.Students.Add(student);
                db.SaveChanges();
                Console.WriteLine("Студент у БД!");
            }
            else
            {
                Console.WriteLine("Не додаємо!");
            }

            foreach (Student st in db.Students)
            {
                Console.WriteLine($"Ім'я: {st.LastName} {st.FirstName}\nВік: {st.Age}");
            }
        }
    }
}
