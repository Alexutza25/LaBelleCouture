using Try.Domain;
using Try.Repository.FavouriteRepository;

namespace Try.Service.FavouriteService;

public class FavouriteService : IFavouriteService
{
    private readonly IFavouriteRepository _favouriteRepository;

    public FavouriteService(IFavouriteRepository favouriteRepository)
    {
        _favouriteRepository = favouriteRepository;
    }

    public async Task<IEnumerable<Favourite>> GetAllFavourites()
    {
        return await _favouriteRepository.GetAll();
    }

    public async Task<IEnumerable<Favourite?>> GetFavouriteById(int userId)
    {
        Console.WriteLine("Intrat în GetFavouriteByUserId");

        var favourites = await _favouriteRepository.GetFavouriteByUserId(userId);

        Console.WriteLine($"FavouriteService a returnat {favourites?.Count()} rezultate");

        return favourites;
    }


    public async Task<Favourite> CreateFavourite(Favourite favourite)
    {
        await _favouriteRepository.Add(favourite);
        return favourite;
    }

    public async Task<Favourite> UpdateFavourite(Favourite favourite)
    {
        await _favouriteRepository.Update(favourite);
        return favourite;
    }

    public async Task<bool> DeleteFavourite(int id)
    {
        var existing = await _favouriteRepository.GetById(id);
        if (existing == null) return false;
        await _favouriteRepository.Delete(id);
        return true;
    }
    
    public async Task<IEnumerable<Favourite>> GetFavouriteByUserId(int userId)
    {
        return await _favouriteRepository.GetFavouriteByUserId(userId);
    }

}