namespace DapperRelations.Entities
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public DateTime DateOfBirth { get; set; }

        public List<Book> Books { get; set; } = new();
    }

}