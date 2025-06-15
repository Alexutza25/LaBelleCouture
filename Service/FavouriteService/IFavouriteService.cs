using Try.Domain;

namespace Try.Service.FavouriteService;

public interface IFavouriteService
{
    Task<IEnumerable<Favourite>> GetAllFavourites();
    Task<IEnumerable<Favourite?>> GetFavouriteById(int id);
    Task<Favourite> CreateFavourite(Favourite favourite);
    Task<Favourite> UpdateFavourite(Favourite favourite);
    Task<bool> DeleteFavourite(int id);
    Task<IEnumerable<Favourite>> GetFavouriteByUserId(int userId);

}