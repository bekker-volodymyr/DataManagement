namespace DapperBulkOperationsMusic.Entities;

public class Song
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ArtistId { get; set; }
    public string Title { get; set; }
}