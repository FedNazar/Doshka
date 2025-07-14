/*
 * Doshka
 * Domain Layer Tests
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

namespace Doshka.Domain.Tests
{
    public class LeaderboardEntryTests
    {
        [Test]
        public void Create_WithAllDetails()
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
                Score = 324432
            };

            Assert.Multiple(() => 
            {
                Assert.That(entry.LeaderboardId, Is.EqualTo(1));
                Assert.That(entry.Leaderboard, Is.EqualTo(leaderboard));
                Assert.That(entry.PlayerId, Is.EqualTo("1"));
                Assert.That(entry.Score, Is.EqualTo(324432));
            });
        }

        [Test]
        public void Create_WithScoreOutOfDefaultRange_Fail()
        {
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                LeaderboardEntry entry = new()
                {
                    LeaderboardId = 1,
                    Leaderboard = new() { Name = "Test", NumOfTopScores = 10 },
                    PlayerId = "1",
                    Score = -1
                };
            });
        }

        [Test]
        public void Create_WithScoreEqualToMin()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                MinScore = 10,
                MaxScore = 1000,
                NumOfTopScores = 10
            };

            Assert.DoesNotThrow(() =>
            {
                LeaderboardEntry entry = new()
                {
                    LeaderboardId = 1,
                    Leaderboard = leaderboard,
                    PlayerId = "1",
                    Score = 10
                };
            });
        }

        [Test]
        public void Create_WithScoreEqualToMax()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                MinScore = 10,
                MaxScore = 1000,
                NumOfTopScores = 10
            };

            Assert.DoesNotThrow(() =>
            {
                LeaderboardEntry entry = new()
                {
                    LeaderboardId = 1,
                    Leaderboard = leaderboard,
                    PlayerId = "1",
                    Score = 1000
                };
            });
        }

        [Test]
        public void Create_WithScoreWithinRange()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                MinScore = 10,
                MaxScore = 1000,
                NumOfTopScores = 10
            };

            Assert.DoesNotThrow(() =>
            {
                LeaderboardEntry entry = new()
                {
                    LeaderboardId = 1,
                    Leaderboard = leaderboard,
                    PlayerId = "1",
                    Score = 500
                };
            });
        }

        [Test]
        public void Create_WithScoreLessThanMin_Fail()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                MinScore = 10,
                MaxScore = 1000,
                NumOfTopScores = 10
            };

            Assert.Catch<ArgumentException>(() =>
            {
                LeaderboardEntry entry = new()
                {
                    LeaderboardId = 1,
                    Leaderboard = leaderboard,
                    PlayerId = "1",
                    Score = 9
                };
            });
        }

        [Test]
        public void Create_WithScoreGreaterThanMax_Fail()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                MinScore = 10,
                MaxScore = 1000,
                NumOfTopScores = 10
            };

            Assert.Catch<ArgumentException>(() =>
            {
                LeaderboardEntry entry = new()
                {
                    LeaderboardId = 1,
                    Leaderboard = leaderboard,
                    PlayerId = "1",
                    Score = 1001
                };
            });
        }

        [Test]
        public void Edit_ScoreWithinRange()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                MinScore = 10,
                MaxScore = 1000,
                NumOfTopScores = 10
            };

            LeaderboardEntry entry = new()
            {
                LeaderboardId = 1,
                Leaderboard = leaderboard,
                PlayerId = "1",
                Score = 500
            };

            Assert.DoesNotThrow(() => entry.Score = 200);
        }

        [Test]
        public void Edit_ScoreEqualToMax()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                MinScore = 10,
                MaxScore = 1000,
                NumOfTopScores = 10
            };

            LeaderboardEntry entry = new()
            {
                LeaderboardId = 1,
                Leaderboard = leaderboard,
                PlayerId = "1",
                Score = 500
            };

            Assert.DoesNotThrow(() => entry.Score = 1000);
        }

        [Test]
        public void Edit_ScoreEqualToMin()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                MinScore = 10,
                MaxScore = 1000,
                NumOfTopScores = 10
            };

            LeaderboardEntry entry = new()
            {
                LeaderboardId = 1,
                Leaderboard = leaderboard,
                PlayerId = "1",
                Score = 500
            };

            Assert.DoesNotThrow(() => entry.Score = 10);
        }

        [Test]
        public void Edit_ScoreLessThanMin_Fail()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                MinScore = 10,
                MaxScore = 1000,
                NumOfTopScores = 10
            };

            LeaderboardEntry entry = new()
            {
                LeaderboardId = 1,
                Leaderboard = leaderboard,
                PlayerId = "1",
                Score = 500
            };

            Assert.Catch<ArgumentOutOfRangeException>(() => entry.Score = 9);
        }

        [Test]
        public void Edit_ScoreGreaterThanMax_Fail()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                MinScore = 10,
                MaxScore = 1000,
                NumOfTopScores = 10
            };

            LeaderboardEntry entry = new()
            {
                LeaderboardId = 1,
                Leaderboard = leaderboard,
                PlayerId = "1",
                Score = 500
            };

            Assert.Catch<ArgumentOutOfRangeException>(() => entry.Score = 1001);
        }
    }
}
