

namespace MoneyBot.DataAccessLayer.Models
{
    public class Category
    {
        public Guid Id { get; set; }
        public  ICollection<Transaction> Transactions { get; set; }
        public ICollection<User> Users { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
