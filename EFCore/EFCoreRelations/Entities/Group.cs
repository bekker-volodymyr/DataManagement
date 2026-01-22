namespace EFCoreRelations.Entities
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Навігаційна властивість
        public List<Student> Students { get; set; } = new();
    }
}
