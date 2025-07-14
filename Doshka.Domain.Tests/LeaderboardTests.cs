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
    public class LeaderboardTests
    {
        [Test]
        public void Create_WithAllDetails()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                MinScore = 0,
                MaxScore = 100000,
                NumOfTopScores = 100
            };

            Assert.Multiple(() =>
            {
                Assert.That(leaderboard.Name, Is.EqualTo("Test"));
                Assert.That(leaderboard.MinScore, Is.EqualTo(0));
                Assert.That(leaderboard.MaxScore, Is.EqualTo(100000));
                Assert.That(leaderboard.NumOfTopScores, Is.EqualTo(100));
            });
        }

        [Test]
        public void Create_WithDefaultMinAndMaxScores()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                NumOfTopScores = 100
            };

            Assert.Multiple(() =>
            {
                Assert.That(leaderboard.Name, Is.EqualTo("Test"));
                Assert.That(leaderboard.MinScore, Is.EqualTo(0));
                Assert.That(leaderboard.MaxScore, Is.EqualTo(long.MaxValue));
                Assert.That(leaderboard.NumOfTopScores, Is.EqualTo(100));
            });
        }

        [Test]
        public void Create_WithDefaultMaxScore()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                MaxScore = 1000,
                NumOfTopScores = 100
            };

            Assert.Multiple(() =>
            {
                Assert.That(leaderboard.Name, Is.EqualTo("Test"));
                Assert.That(leaderboard.MinScore, Is.EqualTo(0));
                Assert.That(leaderboard.MaxScore, Is.EqualTo(1000));
                Assert.That(leaderboard.NumOfTopScores, Is.EqualTo(100));
            });
        }

        [Test]
        public void Create_WithDefaultMinScore()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                MinScore = 1000,
                NumOfTopScores = 100
            };

            Assert.Multiple(() =>
            {
                Assert.That(leaderboard.Name, Is.EqualTo("Test"));
                Assert.That(leaderboard.MinScore, Is.EqualTo(1000));
                Assert.That(leaderboard.MaxScore, Is.EqualTo(long.MaxValue));
                Assert.That(leaderboard.NumOfTopScores, Is.EqualTo(100));
            });
        }

        [Test]
        public void Create_WithDefaultMinScore_EqualToDefaultMax_Fail()
        {
            Assert.Catch<ArgumentException>(() =>
            {
                Leaderboard leaderboard = new()
                {
                    Name = "Test",
                    MinScore = long.MaxValue,
                    NumOfTopScores = 100
                };
            });
        }

        [Test]
        public void Create_WithDefaultMaxScore_EqualToDefaultMin_Fail()
        {
            Assert.Catch<ArgumentException>(() =>
            {
                Leaderboard leaderboard = new()
                {
                    Name = "Test",
                    MinScore = long.MaxValue,
                    NumOfTopScores = 100
                };
            });
        }

        [Test]
        public void Create_WithEqualMinAndMaxScores_Fail()
        {
            Assert.Catch<ArgumentException>(() =>
            {
                Leaderboard leaderboard = new()
                {
                    Name = "Test",
                    MinScore = 100,
                    MaxScore = 100,
                    NumOfTopScores = 100
                };
            });
        }

        [Test]
        public void Create_WithNumOfTopScoresLessThanOne_Fail()
        {
            Assert.Multiple(() =>
            {
                Assert.Catch<ArgumentException>(() =>
                {
                    Leaderboard leaderboard = new()
                    {
                        Name = "Test",
                        NumOfTopScores = 0
                    };
                });

                Assert.Catch<ArgumentException>(() =>
                {
                    Leaderboard leaderboard = new()
                    {
                        Name = "Test",
                        NumOfTopScores = -1
                    };
                });
            });
        }

        [Test]
        public void Edit_MinScore()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                MinScore = 0,
                MaxScore = 1000,
                NumOfTopScores = 10
            };

            leaderboard.MinScore = 100;
            Assert.That(leaderboard.MinScore, Is.EqualTo(100));
        }

        [Test]
        public void Edit_MinScore_EqualToMaxScore_Fail()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                MinScore = 0,
                MaxScore = 1000,
                NumOfTopScores = 10
            };

            Assert.Catch<ArgumentException>(() => leaderboard.MinScore = 1000);
        }

        [Test]
        public void Edit_MinScore_GreaterThanMaxScore_Fail()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                MinScore = 0,
                MaxScore = 1000,
                NumOfTopScores = 10
            };

            Assert.Catch<ArgumentException>(() => leaderboard.MinScore = 1001);
        }

        [Test]
        public void Edit_MaxScore()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                MinScore = 0,
                MaxScore = 1000,
                NumOfTopScores = 10
            };

            leaderboard.MaxScore = 100000;
            Assert.That(leaderboard.MaxScore, Is.EqualTo(100000));
        }

        [Test]
        public void Edit_MaxScore_EqualToMinScore_Fail()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                MinScore = 1000,
                MaxScore = 10000,
                NumOfTopScores = 10
            };

            Assert.Catch<ArgumentException>(() => leaderboard.MaxScore = 1000);
        }

        [Test]
        public void Edit_MaxScore_LessThanMinScore_Fail()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                MinScore = 100,
                MaxScore = 1000,
                NumOfTopScores = 10
            };

            Assert.Catch<ArgumentException>(() => leaderboard.MaxScore = 99);
        }

        [Test]
        public void Edit_NumOfTopScores()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                MinScore = 0,
                MaxScore = 1000,
                NumOfTopScores = 10
            };

            leaderboard.NumOfTopScores = 100;
            Assert.That(leaderboard.NumOfTopScores, Is.EqualTo(100));
        }

        [Test]
        public void Edit_NumOfTopScores_LessThanOne_Fail()
        {
            Leaderboard leaderboard = new()
            {
                Name = "Test",
                MinScore = 0,
                MaxScore = 1000,
                NumOfTopScores = 10
            };

            Assert.Catch<ArgumentException>(() => leaderboard.NumOfTopScores = 0);
        }
    }
}
