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
using Doshka.API.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Doshka.API.Services
{
    public class LeaderboardEntryService : ILeaderboardEntryService
    {
        private IDistributedCache _cache;

        private readonly DistributedCacheEntryOptions CACHE_OPTIONS_TOPN = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        };
        private readonly DistributedCacheEntryOptions CACHE_OPTIONS_ENTRIES = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
        };

        private ILeaderboardEntryRepository _leaderboardEntryRepository;
        private ILeaderboardRepository _leaderboardRepository;
        private IUserRepository _userRepository;

        public LeaderboardEntryService(ILeaderboardEntryRepository leaderboardEntryRepository,
            IUserRepository userRepository, ILeaderboardRepository leaderboardRepository,
            IDistributedCache cache)
        {
            _leaderboardEntryRepository = leaderboardEntryRepository;
            _userRepository = userRepository;
            _leaderboardRepository = leaderboardRepository;
            _cache = cache;
        }

        public async Task<int> Submit(System.Security.Claims.ClaimsPrincipal User,
            LeaderboardEntryNewDto leaderboardEntryDto)
        {
            IdentityUser? user = await _userRepository.GetCurrentUserAsync(User);
            if (user == null) throw new UnauthorizedAccessException();

            LeaderboardEntry? entry = await _leaderboardEntryRepository.
                GetByPlayerIdAsync(user.Id, leaderboardEntryDto.LeaderboardId);

            Leaderboard? leaderboard = await _leaderboardRepository.
                GetByIdAsync(leaderboardEntryDto.LeaderboardId);

            if (leaderboard == null)
            {
                throw new KeyNotFoundException();
            }

            if (entry == null)
            {
                entry = new()
                {
                    LeaderboardId = leaderboardEntryDto.LeaderboardId,
                    Leaderboard = leaderboard,
                    PlayerId = user.Id,
                    Score = leaderboardEntryDto.Score,
                };
                await _leaderboardEntryRepository.AddAsync(entry);
            }
            else
            {
                entry.Score = leaderboardEntryDto.Score;
                await _leaderboardEntryRepository.UpdateAsync(entry);

                await _cache.RemoveAsync($"leaderboardEntry_{entry.Id}");
                await _cache.RemoveAsync($"leaderboardEntry_{leaderboard.Id}_{entry.PlayerId}");
            }

            if ((await GetRankAsync(entry)) <= leaderboard.NumOfTopScores)
            {
                await _cache.RemoveAsync($"{leaderboard.Id}_top");
            }

            return entry.Id;
        }

        public async Task DeleteAsync(System.Security.Claims.ClaimsPrincipal User, 
            int id)
        {
            IdentityUser? user = await _userRepository.GetCurrentUserAsync(User);
            if (user == null) throw new UnauthorizedAccessException();

            LeaderboardEntry? entry = await _leaderboardEntryRepository.GetByIdAsync(id);
            if (entry == null) throw new KeyNotFoundException();

            if (entry.PlayerId != user.Id && !User.IsInRole("Admin"))
                throw new UnauthorizedAccessException();

            if ((await GetRankAsync(entry)) <= entry.Leaderboard.NumOfTopScores)
            {
                await _cache.RemoveAsync($"{entry.Leaderboard.Id}_top");
            }

            await _leaderboardEntryRepository.DeleteAsync(entry);

            await _cache.RemoveAsync($"leaderboardEntry_{entry.Id}");
            await _cache.RemoveAsync($"leaderboardEntry_{entry.LeaderboardId}_{entry.PlayerId}");
        }

        private async Task<LeaderboardEntryExistingDto?> GetCachedEntryAsync(string key)
        {
            string? cachedEntryData = await _cache.GetStringAsync(key);
            if (cachedEntryData != null)
            {
                LeaderboardEntryExistingDto? cachedEntry =
                    JsonSerializer.Deserialize<LeaderboardEntryExistingDto>(
                    cachedEntryData);

                if (cachedEntry != null)
                {
                    Leaderboard? leaderboard = await _leaderboardRepository.
                        GetByIdAsync(cachedEntry.LeaderboardId);
                    if (leaderboard == null) throw new KeyNotFoundException();

                    cachedEntry.Rank = await GetRankAsync(new LeaderboardEntry()
                    {
                        LeaderboardId = cachedEntry.LeaderboardId,
                        Leaderboard = leaderboard,
                        PlayerId = cachedEntry.PlayerId,
                        Score = cachedEntry.Score
                    });
                    return cachedEntry;
                }
            }

            return null;
        }

        private async Task<LeaderboardEntryExistingDto> LeaderboardEntryToExistingDto(
            LeaderboardEntry entry)
        {
            LeaderboardEntryExistingDto dto = new()
            {
                LeaderboardId = entry.LeaderboardId,
                PlayerId = entry.PlayerId,
                Score = entry.Score,
                Rank = await GetRankAsync(entry)
            };

            return dto;
        }

        private LeaderboardEntryExistingDto LeaderboardEntryToExistingDtoWithNoRank(
            LeaderboardEntry entry)
        {
            LeaderboardEntryExistingDto dto = new()
            {
                LeaderboardId = entry.LeaderboardId,
                PlayerId = entry.PlayerId,
                Score = entry.Score,
                Rank = null
            };

            return dto;
        }

        public async Task<LeaderboardEntryExistingDto> GetByIdAsync(int id)
        {
            string cacheKey = $"leaderboardEntry_{id}";
            LeaderboardEntryExistingDto? cachedEntry = await GetCachedEntryAsync(
                cacheKey);
            if (cachedEntry != null) return cachedEntry;

            LeaderboardEntry? entry = await _leaderboardEntryRepository.GetByIdAsync(id);
            if (entry == null) throw new KeyNotFoundException();

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(
                LeaderboardEntryToExistingDtoWithNoRank(entry)),
                CACHE_OPTIONS_ENTRIES);

            return await LeaderboardEntryToExistingDto(entry);
        }

        public async Task<LeaderboardEntryExistingDto> GetByPlayerIdAsync(
            string playerId, int leaderboardId)
        {
            string cacheKey = $"leaderboardEntry_{leaderboardId}_{playerId}";
            LeaderboardEntryExistingDto? cachedEntry = await GetCachedEntryAsync(
                cacheKey);
            if (cachedEntry != null) return cachedEntry;

            LeaderboardEntry? entry = await _leaderboardEntryRepository.GetByPlayerIdAsync(
                playerId, leaderboardId);
            if (entry == null) throw new KeyNotFoundException();

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(
                LeaderboardEntryToExistingDtoWithNoRank(entry)),
                CACHE_OPTIONS_ENTRIES);

            return await LeaderboardEntryToExistingDto(entry);
        }

        public async Task<List<LeaderboardEntryExistingDto>> GetTopNAsync(
            Leaderboard leaderboard)
        {
            string cacheKey = $"{leaderboard.Id}_top";
            string? cachedData = await _cache.GetStringAsync(cacheKey);

            List<LeaderboardEntryExistingDto>? topList;

            if (cachedData != null)
            {
                topList = JsonSerializer.Deserialize
                    <List<LeaderboardEntryExistingDto>>(cachedData);

                if (topList != null) return topList;
            }

            List<LeaderboardEntry> entries = await _leaderboardEntryRepository.
                GetTopNAsync(leaderboard);

            topList = [];
            foreach (LeaderboardEntry entry in entries)
            {
                topList.Add(LeaderboardEntryToExistingDtoWithNoRank(entry));
            }

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(topList),
                CACHE_OPTIONS_TOPN);

            return topList;
        }

        public async Task<int> GetRankAsync(int leaderboardId,
            string playerId)
        {
            LeaderboardEntry? entry = await _leaderboardEntryRepository.
                GetByPlayerIdAsync(playerId, leaderboardId);
            if (entry == null) throw new KeyNotFoundException();

            return await GetRankAsync(entry);
        }

        public async Task<int> GetRankAsync(LeaderboardEntry entry)
        {
            return await _leaderboardEntryRepository.GetRankAsync(entry);
        }
    }
}
