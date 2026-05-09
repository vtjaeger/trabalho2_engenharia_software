using Microsoft.EntityFrameworkCore;
using trabalho2.Data;
using trabalho2.Domain;

namespace trabalho2.Repositories
{
    public class UserLogRepository
    {
        private readonly ApplicationDbContext _context;

        public UserLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserLog log)
        {
            _context.UserLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<List<UserLog>> GetAllAsync()
        {
            return await _context.UserLogs.ToListAsync();
        }
    }
}
