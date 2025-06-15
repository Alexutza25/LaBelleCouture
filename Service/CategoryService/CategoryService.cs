using Try.Domain;
using Try.DTO;
using Try.Repository;
using Try.Repository.CategoryRepository;

namespace Try.Service.CategoryService;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    
    public async Task<IEnumerable<Category>> GetAllCategories()
    {
        return await _categoryRepository.GetAll();
    }

    public async Task<Category?> GetCategoryById(int id)
    {
        return await _categoryRepository.GetById(id);
    }

    public async Task<Category> CreateCategory(CategoryDto dto)
    {
        var category = new Category
        {
            Name = dto.Name,
        };

        await _categoryRepository.Add(category);
        return category;
    }

    public async Task<Category> UpdateCategory(Category category)
    {
       
        var existing = await _categoryRepository.GetById(category.CategoryId);
        if (existing == null) throw new Exception("Category not found"); 
        existing.Name = category.Name;
        await _categoryRepository.Update(existing);
        return existing;
        

    }

    public async Task<bool> DeleteCategory(int id)
    {
        var existing = await _categoryRepository.GetById(id);
        if (existing == null) return false;
        await _categoryRepository.Delete(id);
        return true;
    }
}