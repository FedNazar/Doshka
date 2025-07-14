/*
 * Doshka
 * API Tests
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
using Doshka.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Doshka.API.Services;
using Microsoft.Extensions.Caching.Distributed;
using Moq;

namespace Doshka.API.Tests
{
    public class LeaderboardServiceTests
    {
        private DbContextOptions<ApplicationDbContext> _options;
        private ApplicationDbContext _dbContext;
        private ILeaderboardRepository _leaderboardRepository;
        private ILeaderboardService _leaderboardService;

        private Mock<IDistributedCache> _distributedCacheMock;

        [SetUp]
        public void SetUp()
        {
            _options = new DbContextOptionsBuilder
                <ApplicationDbContext>().UseInMemoryDatabase
                ("DoshkaDb").Options;
            _dbContext = new ApplicationDbContext(_options);
            _leaderboardRepository = new LeaderboardRepository(_dbContext);

            _distributedCacheMock = new Mock<IDistributedCache>();
            _distributedCacheMock.Setup(x => x.RemoveAsync(It.IsAny<string>(),
                default)).Verifiable();

            _leaderboardService = new LeaderboardService(_leaderboardRepository,
                _distributedCacheMock.Object);
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

            await _leaderboardService.AddAsync(leaderboard);

            const int id = 1;
            Leaderboard? resultFromRepo = await _leaderboardRepository.GetByIdAsync(id);

            Assert.Multiple(() =>
            {
                Assert.That(resultFromRepo, Is.Not.Null);
                Assert.That(resultFromRepo.Name, Is.EqualTo("Test"));
                Assert.That(resultFromRepo.NumOfTopScores, Is.EqualTo(10));
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

            await _leaderboardService.AddAsync(leaderboard);

            const int id = 1;
            Leaderboard? resultFromRepo = await _leaderboardService.
                GetByIdAsync(id);

            Assert.Multiple(() =>
            {
                Assert.That(resultFromRepo, Is.Not.Null);
                Assert.That(resultFromRepo.Name, Is.EqualTo("Test"));
                Assert.That(resultFromRepo.NumOfTopScores, Is.EqualTo(10));
            });
        }

        [Test]
        public void GetAsync_ByInvalidId_Fail()
        {
            Assert.CatchAsync<KeyNotFoundException>(() => _leaderboardService.
                GetByIdAsync(1));
        }

        [Test]
        public async Task UpdateAsync()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                NumOfTopScores = 10
            };

            await _leaderboardService.AddAsync(leaderboard);

            const int id = 1;
            Leaderboard? resultFromRepoBefore = await
                _leaderboardService.GetByIdAsync(id);
            resultFromRepoBefore.Name = "Test (Edited)";
            resultFromRepoBefore.NumOfTopScores = 100;
            resultFromRepoBefore.MinScore = 10;
            resultFromRepoBefore.MaxScore = 300000;

            await _leaderboardService.UpdateAsync(resultFromRepoBefore);

            Leaderboard? resultFromRepoAfter = await _leaderboardService.
                GetByIdAsync(resultFromRepoBefore.Id);

            Assert.Multiple(() =>
            {
                Assert.That(resultFromRepoAfter, Is.Not.Null);
                Assert.That(resultFromRepoAfter.Name, Is.EqualTo("Test (Edited)"));
                Assert.That(resultFromRepoAfter.NumOfTopScores, Is.EqualTo(100));
                Assert.That(resultFromRepoAfter.MinScore, Is.EqualTo(10));
                Assert.That(resultFromRepoAfter.MaxScore, Is.EqualTo(300000));
            });
        }

        [Test]
        public void UpdateAsync_InvalidId_Fail()
        {
            Leaderboard leaderboard = new()
            {
                Id = 1,
                Name = "Test",
                NumOfTopScores = 10
            };

            Assert.CatchAsync(async () => await _leaderboardService.
                UpdateAsync(leaderboard));
        }

        [Test]
        public async Task DeleteAsync()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                NumOfTopScores = 10
            };

            await _leaderboardService.AddAsync(leaderboard);

            const int id = 1;
            await _leaderboardService.DeleteAsync(id);

            Leaderboard? resultFromServiceAfter = await _leaderboardRepository.
                GetByIdAsync(id);

            Assert.That(resultFromServiceAfter, Is.Null);
        }

        [Test]
        public void DeleteAsync_InvalidId_Fail()
        {
            Leaderboard leaderboard = new()
            {
                Id = 1,
                Name = "Test",
                NumOfTopScores = 10
            };

            Assert.CatchAsync(async () => await _leaderboardService.
                DeleteAsync(leaderboard.Id));
        }
    }
}
