namespace EFCoreRelations.Entities
{
    public class Teacher
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Salary { get; set; }

        // Багато до багатьох
        public List<Subject> Subjects { get; set; } = new();
    }
}
