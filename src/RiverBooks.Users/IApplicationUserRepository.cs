namespace RiverBooks.Users;

public interface IApplicationUserRepository
{
  Task<ApplicationUser> GetUserWithCartByEmailAsync(string email);
  Task SaveChangesAsync();
}
