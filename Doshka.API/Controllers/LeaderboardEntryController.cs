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

using Doshka.API.DTOs;
using Doshka.API.Filters;
using Doshka.API.Services;
using Doshka.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Doshka.API.Controllers
{
    [ApiController]
    [Authorize]
    [AppExceptionFilter]
    public class LeaderboardEntryController : ControllerBase
    {
        private ILeaderboardEntryService _leaderboardEntryService;
        private ILeaderboardService _leaderboardService;

        public LeaderboardEntryController(
            ILeaderboardEntryService leaderboardEntryService,
            ILeaderboardService leaderboardService)
        {
            _leaderboardEntryService = leaderboardEntryService;
            _leaderboardService = leaderboardService;
        }

        /// <summary>
        /// Get information about an entry with the specified ID.
        /// </summary>
        /// <param name="id">Entry ID.</param>
        /// <returns>Leaderboard entry information.</returns>
        /// <response code="200">Success.</response>
        /// <response code="404">Entry was not found.</response>
        /// <response code="404">Unauthorized user.</response>
        [HttpGet]
        [Route("leaderboards/entries/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LeaderboardEntryExistingDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LeaderboardEntryExistingDto>> Get(int id)
        {
            return Ok(await _leaderboardEntryService.GetByIdAsync(id));
        }

        /// <summary>
        /// Get information about the specified player's entry
        /// in the specific leaderboard.
        /// </summary>
        /// <param name="leaderboardId">Leaderboard ID.</param>
        /// <param name="playerId">Player ID.</param>
        /// <returns>Leaderboard entry information.</returns>
        /// <response code="200">Success.</response>
        /// <response code="404">Entry was not found.</response>
        /// <response code="404">Unauthorized user.</response>
        [HttpGet]
        [Route("leaderboards/{leaderboardId}/entries/{playerId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LeaderboardEntryExistingDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LeaderboardEntryExistingDto>> Get(int leaderboardId,
            string playerId)
        {
            return Ok(await _leaderboardEntryService.GetByPlayerIdAsync(playerId, 
                leaderboardId));
        }

        /// <summary>
        /// Get the top N entries in the specified leaderboard.
        /// N is the "NumOfTopScores" property of the leaderboard.
        /// </summary>
        /// <param name="leaderboardId">Leaderboard ID.</param>
        /// <returns>Top N entries.</returns>
        /// <response code="200">Success.</response>
        /// <response code="404">Leaderboard was not found.</response>
        /// <response code="404">Unauthorized user.</response>
        [HttpGet]
        [Route("leaderboards/{leaderboardId}/entries/top")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<LeaderboardEntryExistingDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<LeaderboardEntryExistingDto>>> GetTopN(
            int leaderboardId)
        {
            Leaderboard leaderboard = await _leaderboardService.
                GetByIdAsync(leaderboardId);
            return Ok(await _leaderboardEntryService.GetTopNAsync(leaderboard));
        }

        /// <summary>
        /// Create an entry or update an existing one.
        /// </summary>
        /// <param name="entryDto">Entry information.</param>
        /// <returns>Entry ID.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad input.</response>
        /// <response code="404">Leaderboard was not found.</response>
        /// <response code="404">Unauthorized user.</response>
        [HttpPost]
        [Route("leaderboards/entries")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Post(LeaderboardEntryNewDto entryDto)
        {
            if (!ModelState.IsValid) return BadRequest();
            return Ok(await _leaderboardEntryService.Submit(User, entryDto));
        }

        /// <summary>
        /// Delete an entry. Players can only delete their own entries.
        /// Admins can delete everyone else's entries.
        /// </summary>
        /// <param name="id">Entry ID.</param>
        /// <returns>Nothing.</returns>
        /// <response code="200">Success.</response>
        /// <response code="404">Leaderboard was not found.</response>
        /// <response code="403">Attempt to delete someone else's entry by the player.</response>
        /// <response code="404">Unauthorized user.</response>
        [HttpDelete]
        [Route("leaderboards/entries")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            await _leaderboardEntryService.DeleteAsync(User, id);
            return Ok();
        }

        /// <summary>
        /// Get player's rank in the specified leaderboard.
        /// </summary>
        /// <param name="leaderboardId">Leaderboard ID.</param>
        /// <param name="playerId">Player ID.</param>
        /// <returns>Player's rank in the leaderboard.</returns>
        /// <response code="200">Success.</response>
        /// <response code="404">Leaderboard or player were not found.</response>
        /// <response code="404">Unauthorized user.</response>
        [HttpGet]
        [Route("leaderboards/{leaderboardId}/ranks/{playerId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<int>> GetRank(int leaderboardId,
            string playerId)
        {
            return Ok(await _leaderboardEntryService.GetRankAsync(
                leaderboardId, playerId));
        }
    }
}
