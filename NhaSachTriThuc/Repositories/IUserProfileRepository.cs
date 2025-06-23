using NhaSachTriThuc.Models;

namespace NhaSachTriThuc.Repositories
{
    public interface IUserProfileRepository
    {
        Task UpdateAsync(UserProfile user);
    }
}
