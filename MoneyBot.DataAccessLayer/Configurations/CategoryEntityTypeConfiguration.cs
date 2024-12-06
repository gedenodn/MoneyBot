using MoneyBot.DataAccsessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccsessLayer.Configurations
{
    public class CategoryEntityTypeConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);
            builder.HasMany(c => c.Users)
                   .WithMany(u => u.Categories);
            builder.HasMany(c => c.Transactions)
                   .WithOne(u => u.Category)
                   .HasForeignKey(c => c.CategoryId);
        }
    }
}
