namespace EFCoreRelations.Entities
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public float? Scholarship { get; set; }
        // Додаємо унікальне поле
        public string? Email { get; set; }

        // Зовнішній ключ
        public int GroupId { get; set; }
        // Навігаційна властивість
        public Group Group { get; set; }
    }
}
