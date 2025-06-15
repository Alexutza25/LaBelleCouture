using Try.Domain;

namespace Try.Repository.FavouriteRepository;

public interface IFavouriteRepository : IRepository<Favourite>
{
    public Task<IEnumerable<Favourite>> GetFavouriteByUserId(int userId);

}