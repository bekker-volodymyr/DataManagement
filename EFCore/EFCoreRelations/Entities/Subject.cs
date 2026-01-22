namespace EFCoreRelations.Entities
{
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Багато до багатьох
        public List<Teacher> Teachers { get; set; } = new();
    }
}
