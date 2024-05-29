namespace LabelSite.Models
{
    
    public interface IUserRepository
    {
        User GetUserById(string userId); // Получение пользователя по идентификатору
        List<User> GetAllUsers(); // Получение всех пользователей (если требуется)
        // Другие методы, например, добавление, обновление и удаление пользователей
    }
}
