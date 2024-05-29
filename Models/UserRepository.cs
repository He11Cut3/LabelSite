using LabelSite.Infrastructure;

namespace LabelSite.Models
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context; // Замените YourDbContext на ваш контекст базы данных

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public User GetUserById(string userId)
        {
            return _context.Users.FirstOrDefault(u => u.Id == userId);
        }

        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }
    }
}
