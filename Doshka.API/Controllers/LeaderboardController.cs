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
    public class LeaderboardController : ControllerBase
    {
        private ILeaderboardService _leaderboardService;

        public LeaderboardController(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        /// <summary>
        /// Get all available leaderboards.
        /// </summary>
        /// <returns>List of all leaderboards.</returns>
        /// <response code="200">Success.</response>
        /// <response code="404">Unauthorized user. Admin privileges are required.</response>
        [HttpGet]
        [Route("leaderboards")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, 
            Type = typeof(IEnumerable<Leaderboard>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Leaderboard>>> Get()
        {
            return Ok(await _leaderboardService.GetAllAsync());
        }

        /// <summary>
        /// Get information about a specific leaderboard.
        /// </summary>
        /// <param name="id">Leaderboard ID.</param>
        /// <returns>Leaderboard information.</returns>
        /// <response code="200">Success.</response>
        /// <response code="404">Leaderboard was not found.</response>
        /// <response code="404">Unauthorized user. Admin privileges are required.</response>
        [HttpGet]
        [Route("leaderboards/{id}/info")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Leaderboard))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Leaderboard>> Get(int id)
        {
            return Ok(await _leaderboardService.GetByIdAsync(id));
        }

        /// <summary>
        /// Create a leaderboard.
        /// </summary>
        /// <param name="leaderboard">Leaderboard info.</param>
        /// <returns>Nothing.</returns>
        /// <response code="201">Success.</response>
        /// <response code="400">Bad input.</response>
        /// <response code="404">Unauthorized user. Admin privileges are required.</response>
        [HttpPost]
        [Route("leaderboards")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Post(Leaderboard leaderboard)
        {
            if (!ModelState.IsValid) return BadRequest();
            await _leaderboardService.AddAsync(leaderboard);
            return Created();
        }

        /// <summary>
        /// Update a leaderboard.
        /// </summary>
        /// <param name="leaderboard">Leaderboard info.</param>
        /// <returns>Nothing.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad input.</response>
        /// <response code="404">Leaderboard was not found.</response>
        /// <response code="404">Unauthorized user. Admin privileges are required.</response>
        [HttpPut]
        [Route("leaderboards")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Put(Leaderboard leaderboard)
        {
            if (!ModelState.IsValid) return BadRequest();
            await _leaderboardService.UpdateAsync(leaderboard);
            return Ok();
        }

        /// <summary>
        /// Delete a leaderboard.
        /// </summary>
        /// <param name="id">Leaderboard ID.</param>
        /// <returns>Nothing.</returns>
        /// <response code="200">Success.</response>
        /// <response code="404">Leaderboard was not found.</response>
        /// <response code="404">Unauthorized user. Admin privileges are required.</response>
        [HttpDelete]
        [Route("leaderboards")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            await _leaderboardService.DeleteAsync(id);
            return Ok();
        }
    }
}
