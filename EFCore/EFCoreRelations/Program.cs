using EFCoreRelations.Entities;

namespace EFCoreRelations
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var context = new AppDbContext();

            // Створення групи разом зі студентами
            var group = new Group
            {
                Name = "Група ІТ-103",
                Students = new List<Student>
                {
                    new Student { Name = "Сергій", Age = 19, Scholarship = 1200 },
                    new Student { Name = "Андрій", Age = 20, Scholarship = null }
                }
            };

            context.Groups.Add(group);
            context.SaveChanges();

            // Створення групи разом зі студентами
            group = new Group
            {
                Name = "Група ІТ-101",
                Students = new List<Student>
                {
                    new Student { Name = "Ірина", Age = 19, Scholarship = 1200 },
                    new Student { Name = "Тарас", Age = 20, Scholarship = null }
                }
            };

            context.Groups.Add(group);
            context.SaveChanges();

            // Додавання студента в існуючу групу
            group = context.Groups.FirstOrDefault(g => g.Id == 1); // Або по назві

            if (group != null)
            {
                var student = new Student
                {
                    Name = "Олександр",
                    Age = 21,
                    Scholarship = 1500,
                    GroupId = group.Id
                };

                context.Students.Add(student);
                context.SaveChanges();
            }

            group = context.Groups.FirstOrDefault(g => g.Name == "Група ІТ-101");

            if (group != null)
            {
                Console.WriteLine(group.Name);
                Console.WriteLine($"group.Id: {group.Id}");
                foreach (var student in group.Students)
                {
                    Console.WriteLine(student.Name);
                }
            }
        }
    }
}
