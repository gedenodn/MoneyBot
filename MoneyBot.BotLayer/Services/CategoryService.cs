using MoneyBot.DataAccessLayer;
using MoneyBot.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace MoneyBot.BotLayer.Services
{
    public class CategoryService
    {
        private readonly MoneyBotDbContext _dbContext;

        public CategoryService(MoneyBotDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateCategoryAsync(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                Console.WriteLine("Category name is empty");
                throw new ArgumentException("Category name cannot be empty", nameof(categoryName));
            }

            var categoryExists = await CategoryExistsAsync(categoryName);
            if (categoryExists)
            {
                Console.WriteLine($"Category '{categoryName}' already exists.");
                throw new InvalidOperationException($"Category '{categoryName}' already exists.");
            }

            var newCategory = new Category
            {
                Id = Guid.NewGuid(),
                CategoryName = categoryName.Trim()
            };

            Console.WriteLine($"Creating category '{categoryName}'...");
            _dbContext.Categories.Add(newCategory);
            await _dbContext.SaveChangesAsync();
            Console.WriteLine($"Category '{categoryName}' created successfully.");
        }

        public async Task<bool> CategoryExistsAsync(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                Console.WriteLine("Category name is empty");
                throw new ArgumentException("Category name cannot be empty", nameof(categoryName));
            }

            var exists = await _dbContext.Categories.AnyAsync(c => c.CategoryName == categoryName.Trim());
            Console.WriteLine($"Category '{categoryName}' exists: {exists}");
            return exists;
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            var categories = await _dbContext.Categories
                .OrderBy(c => c.CategoryName)
                .Select(c => c.CategoryName)
                .ToListAsync();

            Console.WriteLine($"Fetched categories: {string.Join(", ", categories)}");
            return categories;
        }
    }
}
