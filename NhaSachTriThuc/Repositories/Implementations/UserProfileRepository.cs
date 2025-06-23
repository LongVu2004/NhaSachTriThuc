using NhaSachTriThuc.Data;
using NhaSachTriThuc.Models;
using Microsoft.EntityFrameworkCore;

namespace NhaSachTriThuc.Repositories.Implementations
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly AppDbContext _context;
        public UserProfileRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task UpdateAsync(UserProfile user)
        {
            _context.UserProfiles.Update(user); // Fix: Use the correct DbSet for UserProfile
            await _context.SaveChangesAsync();
        }
    }
}
