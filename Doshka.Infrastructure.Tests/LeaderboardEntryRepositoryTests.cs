/*
 * Doshka
 * Infrastructure Layer Tests
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
using Doshka.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Doshka.Infrastructure.Tests
{
    public class LeaderboardEntryRepositoryTests
    {
        private DbContextOptions<ApplicationDbContext> _options;
        private ApplicationDbContext _dbContext;

        private Mock<IPlayerService> _playerServiceMock;
        private ILeaderboardEntryRepository _leaderboardEntryRepository;

        [SetUp]
        public void SetUp()
        {
            _options = new DbContextOptionsBuilder
                <ApplicationDbContext>().UseInMemoryDatabase
                ("DoshkaDb").Options;
            _dbContext = new ApplicationDbContext(_options);

            _playerServiceMock = new Mock<IPlayerService>();
            _leaderboardEntryRepository = new LeaderboardEntryRepository(
                _dbContext, _playerServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task AddAsync()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                NumOfTopScores = 10
            };

            LeaderboardEntry entry = new()
            {
                LeaderboardId = 1,
                Leaderboard = leaderboard,
                PlayerId = "1",
                Score = 42000
            };

            await _leaderboardEntryRepository.AddAsync(entry);

            const int id = 1;
            LeaderboardEntry? resultFromDb = _dbContext.LeaderboardEntries.
                SingleOrDefault(x => x.LeaderboardId.Equals(id));

            Assert.Multiple(() =>
            {
                Assert.That(resultFromDb, Is.Not.Null);
                Assert.That(resultFromDb.Leaderboard, Is.EqualTo(leaderboard));
                Assert.That(resultFromDb.PlayerId, Is.EqualTo("1"));
                Assert.That(resultFromDb.Score, Is.EqualTo(42000));
            });
        }

        [Test]
        public async Task GetAsync_ById()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                NumOfTopScores = 10
            };

            LeaderboardEntry entry = new()
            {
                LeaderboardId = 1,
                Leaderboard = leaderboard,
                PlayerId = "1",
                Score = 42000
            };

            await _dbContext.AddAsync(entry);
            await _dbContext.SaveChangesAsync();

            const int id = 1;
            LeaderboardEntry? resultFromRepo = await _leaderboardEntryRepository.
                GetByIdAsync(id);

            Assert.Multiple(() =>
            {
                Assert.That(resultFromRepo, Is.Not.Null);
                Assert.That(resultFromRepo.Leaderboard, Is.EqualTo(leaderboard));
                Assert.That(resultFromRepo.PlayerId, Is.EqualTo("1"));
                Assert.That(resultFromRepo.Score, Is.EqualTo(42000));
            });
        }

        [Test]
        public async Task GetAsync_ByInvalidId_Fail()
        {
            LeaderboardEntry? resultFromRepo = await _leaderboardEntryRepository.
                GetByIdAsync(1);

            Assert.That(resultFromRepo, Is.Null);
        }

        [Test]
        public async Task GetAsync_ByPlayerId()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                NumOfTopScores = 10
            };

            LeaderboardEntry entry = new()
            {
                LeaderboardId = 1,
                Leaderboard = leaderboard,
                PlayerId = "1",
                Score = 42000
            };

            await _dbContext.AddAsync(entry);
            await _dbContext.SaveChangesAsync();

            const string id = "1";
            const int leaderboardId = 1;
            LeaderboardEntry? resultFromRepo = await _leaderboardEntryRepository.
                GetByPlayerIdAsync(id, leaderboardId);

            Assert.Multiple(() =>
            {
                Assert.That(resultFromRepo, Is.Not.Null);
                Assert.That(resultFromRepo.Leaderboard, Is.EqualTo(leaderboard));
                Assert.That(resultFromRepo.PlayerId, Is.EqualTo("1"));
                Assert.That(resultFromRepo.Score, Is.EqualTo(42000));
            });
        }

        [Test]
        public async Task GetAsync_ByInvalidPlayerIdAndLeaderboardId_Fail()
        {
            LeaderboardEntry? resultFromRepo = await _leaderboardEntryRepository.
                GetByPlayerIdAsync("1", 1);

            Assert.That(resultFromRepo, Is.Null);
        }

        [Test]
        public async Task GetAsync_ByInvalidLeaderboardId_Fail()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                NumOfTopScores = 10
            };

            LeaderboardEntry entry = new()
            {
                LeaderboardId = 2,
                Leaderboard = leaderboard,
                PlayerId = "1",
                Score = 42000
            };

            LeaderboardEntry? resultFromRepo = await _leaderboardEntryRepository.
                GetByPlayerIdAsync("1", 2);

            Assert.That(resultFromRepo, Is.Null);
        }

        [Test]
        public async Task GetAsync_ByInvalidPlayerId_Fail()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                NumOfTopScores = 10
            };

            LeaderboardEntry entry = new()
            {
                LeaderboardId = 1,
                Leaderboard = leaderboard,
                PlayerId = "2",
                Score = 42000
            };

            LeaderboardEntry? resultFromRepo = await _leaderboardEntryRepository.
                GetByPlayerIdAsync("2", 1);

            Assert.That(resultFromRepo, Is.Null);
        }

        [Test]
        public async Task UpdateAsync()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                NumOfTopScores = 10
            };

            LeaderboardEntry entry = new()
            {
                LeaderboardId = 1,
                Leaderboard = leaderboard,
                PlayerId = "1",
                Score = 1000
            };

            await _dbContext.AddAsync(entry);
            await _dbContext.SaveChangesAsync();

            const int id = 1;
            LeaderboardEntry? resultFromRepoBefore = await
                _leaderboardEntryRepository.GetByIdAsync(id);
            resultFromRepoBefore.Score = 30000;

            await _leaderboardEntryRepository.UpdateAsync(resultFromRepoBefore);

            LeaderboardEntry? resultFromRepoAfter = await _leaderboardEntryRepository.
                GetByIdAsync(resultFromRepoBefore.Id);

            Assert.Multiple(() =>
            {
                Assert.That(resultFromRepoAfter, Is.Not.Null);
                Assert.That(resultFromRepoAfter.Score, Is.EqualTo(30000));
            });
        }

        [Test]
        public async Task UpdateAsync_InvalidId_Fail()
        {
            Leaderboard leaderboard = new()
            {
                Id = 1,
                Name = "Test",
                NumOfTopScores = 10
            };

            LeaderboardEntry entry = new()
            {
                LeaderboardId = 1,
                Leaderboard = leaderboard,
                PlayerId = "1",
                Score = 1000
            };

            Assert.CatchAsync(async () => await _leaderboardEntryRepository.
                UpdateAsync(entry));
        }

        [Test]
        public async Task DeleteAsync()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                NumOfTopScores = 10
            };

            LeaderboardEntry entry = new()
            {
                LeaderboardId = 1,
                Leaderboard = leaderboard,
                PlayerId = "1",
                Score = 1000
            };

            await _dbContext.AddAsync(entry);
            await _dbContext.SaveChangesAsync();

            const int id = 1;
            LeaderboardEntry? resultFromRepoBefore = await
                _leaderboardEntryRepository.GetByIdAsync(id);

            await _leaderboardEntryRepository.DeleteAsync(resultFromRepoBefore);

            LeaderboardEntry? resultFromRepoAfter = await _leaderboardEntryRepository.
                GetByIdAsync(resultFromRepoBefore.Id);

            Assert.That(resultFromRepoAfter, Is.Null);
        }

        [Test]
        public async Task DeleteAsync_InvalidId_Fail()
        {
            Leaderboard leaderboard = new()
            {
                Id = 1,
                Name = "Test",
                NumOfTopScores = 10
            };

            LeaderboardEntry entry = new()
            {
                LeaderboardId = 1,
                Leaderboard = leaderboard,
                PlayerId = "1",
                Score = 1000
            };

            Assert.CatchAsync(async () => await _leaderboardEntryRepository.
                DeleteAsync(entry));
        }

        
    }
}
