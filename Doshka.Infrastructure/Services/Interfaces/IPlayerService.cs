namespace Doshka.Infrastructure.Services
{
    public interface IPlayerService
    {
        public Task<string> GetNameByIdAsync(string id);
    }
}
