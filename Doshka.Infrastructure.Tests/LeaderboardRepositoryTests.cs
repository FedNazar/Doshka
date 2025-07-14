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
using Microsoft.EntityFrameworkCore;

namespace Doshka.Infrastructure.Tests
{
    public class LeaderboardRepositoryTests
    {
        private DbContextOptions<ApplicationDbContext> _options;
        private ApplicationDbContext _dbContext;
        private ILeaderboardRepository _leaderboardRepository;

        [SetUp]
        public void SetUp()
        {
            _options = new DbContextOptionsBuilder
                <ApplicationDbContext>().UseInMemoryDatabase
                ("DoshkaDb").Options;
            _dbContext = new ApplicationDbContext(_options);
            _leaderboardRepository = new LeaderboardRepository(_dbContext);
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

            await _leaderboardRepository.AddAsync(leaderboard);

            Leaderboard? resultFromDb = _dbContext.Leaderboards.
                SingleOrDefault(x => x.Name.Equals("Test"));

            Assert.Multiple(() =>
            {
                Assert.That(resultFromDb, Is.Not.Null);
                Assert.That(resultFromDb.NumOfTopScores, Is.EqualTo(10));
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

            await _dbContext.AddAsync(leaderboard);
            await _dbContext.SaveChangesAsync();

            const int id = 1;
            Leaderboard? resultFromRepo = await _leaderboardRepository.
                GetByIdAsync(id);

            Assert.Multiple(() =>
            {
                Assert.That(resultFromRepo, Is.Not.Null);
                Assert.That(resultFromRepo.Name, Is.EqualTo("Test"));
                Assert.That(resultFromRepo.NumOfTopScores, Is.EqualTo(10));
            });
        }

        [Test]
        public async Task GetAsync_ByInvalidId_Fail()
        {
            Leaderboard? resultFromRepo = await _leaderboardRepository.
                GetByIdAsync(1);

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

            await _dbContext.AddAsync(leaderboard);
            await _dbContext.SaveChangesAsync();

            const int id = 1;
            Leaderboard? resultFromRepoBefore = await 
                _leaderboardRepository.GetByIdAsync(id);
            resultFromRepoBefore.Name = "Test (Edited)";
            resultFromRepoBefore.NumOfTopScores = 100;
            resultFromRepoBefore.MinScore = 10;
            resultFromRepoBefore.MaxScore = 300000;

            await _leaderboardRepository.UpdateAsync(resultFromRepoBefore);

            Leaderboard? resultFromRepoAfter = await _leaderboardRepository.
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
        public async Task UpdateAsync_InvalidId_Fail()
        {
            Leaderboard leaderboard = new()
            {
                Id = 1,
                Name = "Test",
                NumOfTopScores = 10
            };

            Assert.CatchAsync(async () => await _leaderboardRepository.
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

            await _dbContext.AddAsync(leaderboard);
            await _dbContext.SaveChangesAsync();

            const int id = 1;
            Leaderboard? resultFromRepoBefore = await
                _leaderboardRepository.GetByIdAsync(id);

            await _leaderboardRepository.DeleteAsync(resultFromRepoBefore);

            Leaderboard? resultFromRepoAfter = await _leaderboardRepository.
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

            Assert.CatchAsync(async () => await _leaderboardRepository.
                DeleteAsync(leaderboard));
        }
    }
}
