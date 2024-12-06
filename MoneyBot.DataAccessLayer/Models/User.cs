
namespace MoneyBot.DataAccessLayer.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public long TelegramUserId { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<Category> Categories { get; set; }
    }
}
