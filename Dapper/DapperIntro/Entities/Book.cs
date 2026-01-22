namespace DapperIntro.Entities;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;

    public override string ToString()
    {
        return $"{Id}: {Title} by {Author}";
    }
}