using MoneyBot.DataAccsessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace MoneyBot.DataAccsessLayer
{
    public class MoneyBotDbContext : DbContext
    {
        public MoneyBotDbContext(DbContextOptions<MoneyBotDbContext> options)
            : base(options) 
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
         
        }
    }
}
