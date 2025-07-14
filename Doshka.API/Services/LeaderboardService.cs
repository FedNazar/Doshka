/*
 * Doshka
 * API
 * 
 * (C) 2025 Nazar Fedorenko
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the “Software”), 
 * to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE. 
 */

using Doshka.Domain.Entities;
using Doshka.Infrastructure.Repositories;
using Microsoft.Extensions.Caching.Distributed;

namespace Doshka.API.Services
{
    public class LeaderboardService : ILeaderboardService
    {
        private ILeaderboardRepository _repository;
        private IDistributedCache _cache;

        public LeaderboardService(ILeaderboardRepository repository,
            IDistributedCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task AddAsync(Leaderboard entity)
        {
            await _repository.AddAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            Leaderboard leaderboard = await GetByIdAsync(id);
            await _repository.DeleteAsync(leaderboard);
            await _cache.RemoveAsync($"{leaderboard.Id}_top");
        }

        public async Task<Leaderboard> GetByIdAsync(int id)
        {
            Leaderboard? leaderboard = await _repository.GetByIdAsync(id);
            if (leaderboard == null) throw new KeyNotFoundException();

            return leaderboard;
        }

        public async Task<IEnumerable<Leaderboard>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task UpdateAsync(Leaderboard entity)
        {
            await _repository.UpdateAsync(entity);
        }
    }
}