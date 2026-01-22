namespace DapperBulkOperations.Entities
{
    public class Passport
    {
        public int Id { get; set; }
        public string PassportNumber { get; set; } = default!;
        public int VisitorId { get; set; }

        public Visitor? Visitor { get; set; }

        public override string ToString()
        {
            return $"{PassportNumber}.";
        }
    }
}