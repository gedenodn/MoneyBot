using MoneyBot.DataAccessLayer.Models;

namespace MoneyBot.DataAccessLayer.Repositories;
public interface ICategoryRepository
{
    Task<List<Category>> GetCategoriesByUserIdAsync(Guid userId);
    Task<Category> AddCategoryAsync(Category category);
}