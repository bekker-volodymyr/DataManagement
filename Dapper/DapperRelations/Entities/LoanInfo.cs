namespace DapperRelations.Entities;

public class LoanInfo
{
    public Book Book { get; set; } = null!;
    public DateTime LoanDate { get; set; }
    public DateTime? ReturnDate { get; set; }

}