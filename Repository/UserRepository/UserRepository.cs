using Microsoft.EntityFrameworkCore;
using Try.Domain;

namespace Try.Repository.UserRepository;

public class UserRepository : GenericRepository<User>, IUserRepository 
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<User> GetByEmail(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
    
    public async Task<Address?> GetAddressByUserId(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Address)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        return user?.Address;
    }
    
    public async Task UpdateAddress(Address address)
    {
        _context.Update(address);
        await _context.SaveChangesAsync();
    }


}