using Try.Domain;
using Try.DTO;

namespace Try.Service.UserService;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsers();
    Task<User?> GetUserById(int id);
    Task<User?> GetUserByEmail(string email);
    Task<User> CreateUser(User user);
    Task<bool> DeleteUser(int id);
    Task<User> UpdateUser(int id, UpdateUserDto updatedUser);
    
    Task<User> RegisterUser(SignUpDto dto);
    
    Task<User?> Authenticate(string email, string password);
    
    Task<Address?> GetAddressByUserId(int userId);
    
    Task<bool> AddAdminAsync(CreateAdminDto dto);
    
    Task ChangePassword(int userId, string newPassword);



}