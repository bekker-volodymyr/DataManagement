namespace EFCoreFluentApi.Entities
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        // Унікальне поле
        public string Email { get; set; } = null!;
        public decimal? Scholarship { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; } = null!;

    }
}
