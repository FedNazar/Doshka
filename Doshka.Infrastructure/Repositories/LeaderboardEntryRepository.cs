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
using Doshka.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Doshka.Infrastructure.Repositories
{
    public class LeaderboardEntryRepository : ILeaderboardEntryRepository
    {
        private ApplicationDbContext _dbContext;
        private IPlayerService _playerService;

        public LeaderboardEntryRepository(ApplicationDbContext dbContext,
            IPlayerService playerService)
        {
            _dbContext = dbContext;
            _playerService = playerService;
        }

        public async Task<int> GetRankAsync(LeaderboardEntry entry)
        {
            int betterPlayers = await _dbContext.LeaderboardEntries
                .Where(x => x.Score > entry.Score)
                .Where(x => x.LeaderboardId.Equals(entry.LeaderboardId))
                .CountAsync();

            List<LeaderboardEntry> entriesWithTheSameScore =
                await _dbContext.LeaderboardEntries
                .Where(x => x.Score.Equals(entry.Score))
                .Where(x => x.LeaderboardId.Equals(entry.LeaderboardId))
                .Where(x => !x.PlayerId.Equals(entry.PlayerId))
                .ToListAsync();

            if (entriesWithTheSameScore.Count == 0) return betterPlayers + 1;

            string curPlayerName = await _playerService.
                GetNameByIdAsync(entry.PlayerId);
            string[] playerNames = await Task.WhenAll(
                entriesWithTheSameScore.Select(x => 
                _playerService.GetNameByIdAsync(x.PlayerId)));
            int lexicographicallyLesserPlayers = playerNames.
                Count(name => string.Compare(name, curPlayerName) < 0);

            return betterPlayers + lexicographicallyLesserPlayers + 1;
        }

        public async Task AddAsync(LeaderboardEntry entity)
        {
            await _dbContext.LeaderboardEntries.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(LeaderboardEntry entity)
        {
            _dbContext.LeaderboardEntries.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<LeaderboardEntry?> GetByIdAsync(int id)
        {
            return await _dbContext.LeaderboardEntries.Include(x => x.Leaderboard)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<LeaderboardEntry?> GetByPlayerIdAsync(string playerId,
            int leaderboardId)
        {
            LeaderboardEntry? leaderboardEntry = await _dbContext.LeaderboardEntries
                .Include(x => x.Leaderboard)
                .Where(x => x.PlayerId.Equals(playerId))
                .Where(x => x.LeaderboardId.Equals(leaderboardId))
                .FirstOrDefaultAsync();

            return leaderboardEntry;
        }

        public async Task<List<LeaderboardEntry>> GetTopNAsync(
            Leaderboard leaderboard)
        {
            return await _dbContext.LeaderboardEntries
                .Where(x => x.LeaderboardId.Equals(leaderboard.Id))
                .OrderByDescending(x => x.Score)
                .Take(leaderboard.NumOfTopScores)
                .ToListAsync();
        }

        public async Task UpdateAsync(LeaderboardEntry entity)
        {
            _dbContext.LeaderboardEntries.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
