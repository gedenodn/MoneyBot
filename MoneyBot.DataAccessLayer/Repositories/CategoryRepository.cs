using Microsoft.EntityFrameworkCore;
using MoneyBot.DataAccessLayer;
using MoneyBot.DataAccessLayer.Models;
using MoneyBot.DataAccessLayer.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly MoneyBotDbContext _context;

    public CategoryRepository(MoneyBotDbContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetCategoriesByUserIdAsync(Guid userId)
    {
        return await _context.Categories
            .Where(c => c.Users.Any(u => u.Id == userId))
            .ToListAsync();
    }

    public async Task<Category> AddCategoryAsync(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }
}