/*
 * Doshka
 * Infrastructure Layer
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
using Microsoft.EntityFrameworkCore;

namespace Doshka.Infrastructure.Repositories
{
    public class LeaderboardRepository : ILeaderboardRepository
    {
        private ApplicationDbContext _dbContext;

        public LeaderboardRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Leaderboard entity)
        {
            await _dbContext.Leaderboards.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Leaderboard entity)
        {
            _dbContext.Leaderboards.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Leaderboard>> GetAllAsync()
        {
            return await _dbContext.Leaderboards.ToListAsync();
        }

        public async Task<Leaderboard?> GetByIdAsync(int id)
        {
            return await _dbContext.Leaderboards.FindAsync(id);
        }

        public async Task UpdateAsync(Leaderboard entity)
        {
            try
            {
                _dbContext.Leaderboards.Update(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new KeyNotFoundException();
            }
        }
    }
}
