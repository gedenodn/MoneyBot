
namespace MoneyBot.DataAccsessLayer.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public decimal Sum { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
