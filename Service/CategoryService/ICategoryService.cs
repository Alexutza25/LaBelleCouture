using Try.Domain;
using Try.DTO;

namespace Try.Service.CategoryService;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllCategories();
    Task<Category> GetCategoryById(int id);
    Task<Category> CreateCategory(CategoryDto dto);
    Task<Category> UpdateCategory(Category category);
    Task<bool> DeleteCategory(int id);
}