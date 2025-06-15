using Microsoft.AspNetCore.Identity;
using Try.Domain;
using Try.DTO;
using Try.Repository.UserRepository;

namespace Try.Service.UserService;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<User>> GetAllUsers()
    {
        return await _userRepository.GetAll();
    }

    public async Task<User?> GetUserById(int id)
    {
        return await _userRepository.GetById(id);
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _userRepository.GetByEmail(email);
    }

    public async Task<User> CreateUser(User user)
    {
        try
        {
            await _userRepository.Add(user);
            return user;
        }
        catch (Exception ex)
        { 
            throw new Exception("Error creating user: " + ex.Message, ex);        }
        
    }
    
    public async Task<bool> AddAdminAsync(CreateAdminDto dto)    {
        var user = new User
        {
            Email = dto.Email,
            Password = dto.Password,
            Name = dto.Name,
            Phone = dto.PhoneNumber,
            TypeUser = "Administrator",
            AddressId = 1
        };

        await _userRepository.Add(user);
        return true;
    }


    public async Task<bool> DeleteUser(int id)
    {
        var user = await _userRepository.GetById(id);
        if (user == null)
            return false;

        await _userRepository.Delete(id);
        return true;
    }

    public async Task<User> UpdateUser(int id, UpdateUserDto updatedUser)
    {
        var user = await _userRepository.GetById(id);
        if (user == null) return null;

        var address = await _userRepository.GetAddressByUserId(id);

        user.Name = updatedUser.Name;
        user.Email = updatedUser.Email;
        user.Phone = updatedUser.Phone;

        if (address != null && updatedUser.Address != null)
        {
            address.County = updatedUser.Address.County;
            address.City = updatedUser.Address.City;
            address.Street = updatedUser.Address.Street;
            address.Number = updatedUser.Address.Number;
            address.BuildingEntrance = updatedUser.Address.BuildingEntrance;
            address.Floor = updatedUser.Address.Floor;
            address.ApartmentNumber = updatedUser.Address.ApartmentNumber;
            address.AdditionalDetails = updatedUser.Address.AdditionalDetails;

            await _userRepository.UpdateAddress(address); 
        }

        await _userRepository.Update(user); 
        return user;
    }

    
    public async Task<User> RegisterUser(SignUpDto dto)
    {
        var existingUser = await _userRepository.GetByEmail(dto.Email);
        if (existingUser != null)
            throw new Exception("Email already exists!");
        
        var address = new Address
        {
            County = dto.Address.County,
            City = dto.Address.City,
            Street = dto.Address.Street,
            Number = dto.Address.Number,
            BuildingEntrance = dto.Address.BuildingEntrance,
            Floor = dto.Address.Floor,
            ApartmentNumber = dto.Address.ApartmentNumber,
            AdditionalDetails = dto.Address.AdditionalDetails
        };

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Password = dto.Password,
            Phone = dto.Phone,
            TypeUser = dto.TypeUser,
            Address = address
        };

        await _userRepository.Add(user);
        return user;
    }
    
    public async Task<User?> Authenticate(string email, string password)
    {
        var user = await _userRepository.GetByEmail(email);
        if (user == null)
            return null;

        if (user.Password != password)
            return null;

        return user;
    }
    
    public async Task<Address?> GetAddressByUserId(int userId)
    {
        return await _userRepository.GetAddressByUserId(userId);
    }
    
    public async Task ChangePassword(int userId, string newPassword)
    {
        var user = await _userRepository.GetById(userId);
        if (user == null) return;

        user.Password = newPassword;
        await _userRepository.Update(user);
    }


}