using Try.Repository;
using Try.Repository.ProductVariantRepository;
using Try.Domain;

namespace Try.Repository.ProductVariantRepository;

public class ProductVariantRepository : GenericRepository<ProductVariant>, IProductVariantRepository
{
    public ProductVariantRepository(ApplicationDbContext context) : base(context){}
}