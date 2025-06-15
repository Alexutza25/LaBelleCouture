using Try.Domain;
using System.Threading.Tasks;
namespace Try.Repository.UserRepository;

public interface IUserRepository: IRepository<User>
{
   Task<User> GetByEmail(string email); 
   Task<Address?> GetAddressByUserId(int userId);
   Task UpdateAddress(Address address);

}