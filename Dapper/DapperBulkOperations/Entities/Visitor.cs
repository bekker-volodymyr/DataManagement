namespace DapperBulkOperations.Entities
{
    public class Visitor
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public DateTime DateOfBirth { get; set; }

        public Passport? Passport { get; set; }

        // Навігаційна властивість у Visitor
        public List<LoanInfo> Loans { get; set; } = new();

        public override string ToString()
        {
            return $"{Id}. {Name}. {PhoneNumber}. {DateOfBirth}\nPassport: {Passport}";
        }
    }
}