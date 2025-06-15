using Try.Domain;

namespace Try.Repository.CategoryRepository;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {}
}