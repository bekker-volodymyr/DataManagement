namespace EFCoreIntro.Entities
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int Age { get; set; }
        // Додаємо поле Scholarship
        // float? - nullable тип, вказує EFCore, що поле може бути null
        public float? Scholarship { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}\nІм'я: {LastName} {FirstName}\nВік: {Age}\nScholarship: {(Scholarship is null ? 0 : Scholarship)}";
        }
    }
}
