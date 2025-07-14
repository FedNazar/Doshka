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
using Doshka.Infrastructure.Services;
using Doshka.Infrastructure;
using Doshka.API.Services;
using Doshka.API.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Distributed;

namespace Doshka.API.Tests
{
    public class LeaderboardEntryServiceTests
    {
        private DbContextOptions<ApplicationDbContext> _options;
        private ApplicationDbContext _dbContext;

        private ILeaderboardEntryRepository _leaderboardEntryRepository;
        private ILeaderboardRepository _leaderboardRepository;
        private ILeaderboardEntryService _leaderboardEntryService;

        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IPlayerService> _playerServiceMock;
        private Mock<IDistributedCache> _distributedCacheMock;

        [SetUp]
        public void SetUp()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("DoshkaDb").Options;

            _dbContext = new ApplicationDbContext(_options);
            _playerServiceMock = new Mock<IPlayerService>();
            _userRepositoryMock = new Mock<IUserRepository>();

            _leaderboardEntryRepository = new LeaderboardEntryRepository(_dbContext, 
                _playerServiceMock.Object);
            _leaderboardRepository = new LeaderboardRepository(_dbContext);

            _distributedCacheMock = new Mock<IDistributedCache>();
            _distributedCacheMock.Setup(x => x.GetAsync(It.IsAny<string>(),
                default)).ReturnsAsync((byte[]?)null);
            _distributedCacheMock.Setup(x => x.SetAsync(It.IsAny<string>(),
                It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(),
                default)).Verifiable();
            _distributedCacheMock.Setup(x => x.RemoveAsync(It.IsAny<string>(),
                default)).Verifiable();

            _leaderboardEntryService = new LeaderboardEntryService(
                _leaderboardEntryRepository, _userRepositoryMock.Object,
                _leaderboardRepository, _distributedCacheMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task Submit_NewEntry()
        {
            IdentityUser identityUser = new IdentityUser { Id = "user1" };
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal();

            _userRepositoryMock.Setup(x => x.GetCurrentUserAsync(claimsPrincipal))
                .ReturnsAsync(identityUser);

            Leaderboard leaderboard = new Leaderboard
            {
                Id = 1,
                Name = "Test",
                NumOfTopScores = 10
            };
            await _leaderboardRepository.AddAsync(leaderboard);

            LeaderboardEntryNewDto newDto = new LeaderboardEntryNewDto
            {
                LeaderboardId = leaderboard.Id,
                Score = 100
            };

            int resultId = await _leaderboardEntryService.Submit(claimsPrincipal, newDto);

            LeaderboardEntry? savedEntry = await _leaderboardEntryRepository.GetByIdAsync(resultId);

            Assert.Multiple(() =>
            {
                Assert.That(savedEntry, Is.Not.Null);
                Assert.That(savedEntry.Score, Is.EqualTo(100));
                Assert.That(savedEntry.PlayerId, Is.EqualTo("user1"));
                Assert.That(savedEntry.LeaderboardId, Is.EqualTo(leaderboard.Id));
            });
        }

        [Test]
        public async Task Submit_UpdateEntry()
        {
            IdentityUser identityUser = new IdentityUser { Id = "user1" };
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal();

            _userRepositoryMock.Setup(x => x.GetCurrentUserAsync(claimsPrincipal))
                .ReturnsAsync(identityUser);

            Leaderboard leaderboard = new Leaderboard
            {
                Id = 1,
                Name = "Test",
                NumOfTopScores = 10
            };
            await _leaderboardRepository.AddAsync(leaderboard);

            LeaderboardEntry entry = new LeaderboardEntry
            {
                LeaderboardId = 1,
                Leaderboard = leaderboard,
                PlayerId = "user1",
                Score = 50
            };
            await _leaderboardEntryRepository.AddAsync(entry);

            LeaderboardEntryNewDto newDto = new LeaderboardEntryNewDto
            {
                LeaderboardId = 1,
                Score = 200
            };

            int resultId = await _leaderboardEntryService.Submit(claimsPrincipal, newDto);

            LeaderboardEntry? updatedEntry = await _leaderboardEntryRepository.GetByIdAsync(resultId);

            Assert.Multiple(() =>
            {
                Assert.That(updatedEntry, Is.Not.Null);
                Assert.That(updatedEntry.Id, Is.EqualTo(entry.Id));
                Assert.That(updatedEntry.Score, Is.EqualTo(200));
            });
        }

        [Test]
        public void Submit_UserUnauthorized_Fail()
        {
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal();

            _userRepositoryMock.Setup(x => x.GetCurrentUserAsync(claimsPrincipal))
                .ReturnsAsync((IdentityUser?)null);

            LeaderboardEntryNewDto newDto = new LeaderboardEntryNewDto
            {
                LeaderboardId = 1,
                Score = 100
            };

            Assert.CatchAsync<UnauthorizedAccessException>(() =>
                _leaderboardEntryService.Submit(claimsPrincipal, newDto));
        }

        [Test]
        public async Task Submit_InvalidLeaderboard_Fail()
        {
            IdentityUser identityUser = new IdentityUser { Id = "user1" };
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal();

            _userRepositoryMock.Setup(x => x.GetCurrentUserAsync(claimsPrincipal))
                .ReturnsAsync(identityUser);

            LeaderboardEntryNewDto newDto = new LeaderboardEntryNewDto
            {
                LeaderboardId = 999,
                Score = 100
            };

            Assert.CatchAsync<KeyNotFoundException>(() =>
                _leaderboardEntryService.Submit(claimsPrincipal, newDto));
        }

        [Test]
        public async Task DeleteAsync_ValidUserAndEntry_Admin()
        {
            IdentityUser user = new()
            {
                Id = "user1",
                UserName = "testUser"
            };

            Leaderboard leaderboard = new()
            {
                Name = "TestLeaderboard",
                NumOfTopScores = 10
            };
            await _leaderboardRepository.AddAsync(leaderboard);

            LeaderboardEntry entry = new()
            {
                PlayerId = user.Id,
                LeaderboardId = leaderboard.Id,
                Leaderboard = leaderboard,
                Score = 100
            };
            await _leaderboardEntryRepository.AddAsync(entry);

            ClaimsPrincipal principal = new(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

            _userRepositoryMock.Setup(repo => repo.GetCurrentUserAsync(principal))
                .ReturnsAsync(user);

            await _leaderboardEntryService.DeleteAsync(principal, entry.Id);

            LeaderboardEntry? deletedEntry = await _leaderboardEntryRepository.
                GetByIdAsync(entry.Id);
            Assert.That(deletedEntry, Is.Null);
        }

        [Test]
        public void DeleteAsync_InvalidEntryId()
        {
            ClaimsPrincipal principal = new(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user1"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

            _userRepositoryMock.Setup(repo => repo.GetCurrentUserAsync(principal))
                .ReturnsAsync(new IdentityUser { Id = "user1" });

            Assert.CatchAsync<KeyNotFoundException>(() =>
                _leaderboardEntryService.DeleteAsync(principal, 999));
        }

        [Test]
        public void DeleteAsync_UnauthorizedUser_Fail()
        {
            IdentityUser user = new()
            {
                Id = "user1"
            };

            Leaderboard leaderboard = new()
            {
                Name = "TestLeaderboard",
                NumOfTopScores = 10
            };
            _dbContext.Leaderboards.Add(leaderboard);
            _dbContext.SaveChanges();

            LeaderboardEntry entry = new()
            {
                PlayerId = "differentUser",
                LeaderboardId = leaderboard.Id,
                Leaderboard = leaderboard,
                Score = 50
            };
            _dbContext.LeaderboardEntries.Add(entry);
            _dbContext.SaveChanges();

            ClaimsPrincipal principal = new(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            }, "mock"));

            _userRepositoryMock.Setup(repo => repo.GetCurrentUserAsync(principal))
                .ReturnsAsync(user);

            Assert.CatchAsync<UnauthorizedAccessException>(() =>
                _leaderboardEntryService.DeleteAsync(principal, entry.Id));
        }

        [Test]
        public async Task GetByIdAsync_ValidId()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                NumOfTopScores = 10
            };
            await _leaderboardRepository.AddAsync(leaderboard);

            LeaderboardEntry entry = new()
            {
                PlayerId = "user1",
                LeaderboardId = leaderboard.Id,
                Leaderboard = leaderboard,
                Score = 100,
            };
            await _leaderboardEntryRepository.AddAsync(entry);

            LeaderboardEntryExistingDto dto = await _leaderboardEntryService
                .GetByIdAsync(entry.Id);

            Assert.Multiple(() =>
            {
                Assert.That(dto, Is.Not.Null);
                Assert.That(dto.PlayerId, Is.EqualTo("user1"));
                Assert.That(dto.Score, Is.EqualTo(100));
                Assert.That(dto.LeaderboardId, Is.EqualTo(leaderboard.Id));
            });
        }

        [Test]
        public void GetByIdAsync_InvalidId_Fail()
        {
            Assert.CatchAsync<KeyNotFoundException>(() =>
                _leaderboardEntryService.GetByIdAsync(999));
        }

        [Test]
        public async Task GetByPlayerIdAsync()
        {
            Leaderboard leaderboard = new()
            {
                Name = "TestLeaderboard",
                NumOfTopScores = 10
            };
            await _leaderboardRepository.AddAsync(leaderboard);

            LeaderboardEntry entry = new()
            {
                PlayerId = "player1",
                LeaderboardId = leaderboard.Id,
                Leaderboard = leaderboard,
                Score = 150
            };
            await _leaderboardEntryRepository.AddAsync(entry);

            LeaderboardEntryExistingDto result = await _leaderboardEntryService
                .GetByPlayerIdAsync("player1", leaderboard.Id);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.PlayerId, Is.EqualTo("player1"));
                Assert.That(result.Score, Is.EqualTo(150));
                Assert.That(result.LeaderboardId, Is.EqualTo(leaderboard.Id));
            });
        }

        [Test]
        public void GetByPlayerIdAsync_InvalidPlayerOrLeaderboard_Fail()
        {
            Assert.CatchAsync<KeyNotFoundException>(() =>
                _leaderboardEntryService.GetByPlayerIdAsync("nonexistent", 1));
        }

        [Test]
        public async Task DeleteAsync_PlayersOwnEntry()
        {
            IdentityUser user = new()
            {
                Id = "player123",
                UserName = "playerUser"
            };

            Leaderboard leaderboard = new()
            {
                Name = "Test",
                NumOfTopScores = 10
            };
            await _leaderboardRepository.AddAsync(leaderboard);

            LeaderboardEntry entry = new()
            {
                PlayerId = user.Id,
                LeaderboardId = leaderboard.Id,
                Leaderboard = leaderboard,
                Score = 75
            };
            await _leaderboardEntryRepository.AddAsync(entry);

            ClaimsPrincipal principal = new(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, "Player")
            }, "mock"));

            _userRepositoryMock.Setup(repo => repo.GetCurrentUserAsync(principal))
                .ReturnsAsync(user);

            await _leaderboardEntryService.DeleteAsync(principal, entry.Id);

            LeaderboardEntry? deletedEntry = await _leaderboardEntryRepository.
                GetByIdAsync(entry.Id);
            Assert.That(deletedEntry, Is.Null);
        }
    }
}
