/*
 * Doshka
 * Domain Layer
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

namespace Doshka.Domain.Entities
{
    public class Leaderboard : BaseEntity
    {
        public required string Name { get; set; }

        long _minScore = 0;
        public long MinScore
        {
            get
            {
                return _minScore;
            }
            set
            {
                if (value >= MaxScore) 
                    throw new ArgumentException(
                        "Min. score cannot be greater than or equal to max. score");

                _minScore = value;
            }
        }

        long _maxScore = long.MaxValue;
        public long MaxScore
        {
            get
            {
                return _maxScore;
            }
            set
            {
                if (value <= MinScore)
                    throw new ArgumentException(
                        "Max. score cannot be less than or equal to min. score");

                _maxScore = value;
            } 
        }

        int _numOfTopScores;
        public required int NumOfTopScores
        {
            get
            {
                return _numOfTopScores;
            }
            set
            {
                if (value < 1)
                    throw new ArgumentException(
                        "Number of top scores cannot be less than one");

                _numOfTopScores = value;
            }
        }
    }
}
