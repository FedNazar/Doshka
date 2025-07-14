namespace Doshka.Infrastructure.Services
{
    public interface IUserSeederService
    {
        public Task SeedRolesAsync();
        public Task SeedUsersAsync();
    }
}
