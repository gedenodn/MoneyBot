using Microsoft.EntityFrameworkCore;
using MoneyBot.DataAccessLayer;
using MoneyBot.DataAccessLayer.Models;

namespace MoneyBot.BotLayer.Services
{
    public class ExpenseService
{
    private readonly MoneyBotDbContext _dbContext;

    public ExpenseService(MoneyBotDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddExpenseAsync(string categoryName, decimal amount)
    {
        var category = await _dbContext.Categories
            .FirstOrDefaultAsync(c => c.CategoryName == categoryName);

        if (category == null)
        {
            Console.WriteLine($"Category '{categoryName}' not found");
            throw new Exception("Category not found");
        }

        var newTransaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Sum = amount,
            CategoryId = category.Id
        };

        _dbContext.Transactions.Add(newTransaction);
        await _dbContext.SaveChangesAsync();
        
        Console.WriteLine($"Added expense of {amount:C} to category '{categoryName}'");
    }
}

}
