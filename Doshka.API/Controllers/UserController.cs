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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Doshka.API.Controllers
{
    [ApiController]
    [Authorize]
    [AppExceptionFilter]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Login.
        /// </summary>
        /// <param name="logInDto">Login credentials.</param>
        /// <returns>Nothing.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad input.</response>
        [HttpPost]
        [AllowAnonymous]
        [Route("users/login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> LogIn(UserLogInDto logInDto)
        {
            if (!ModelState.IsValid) return BadRequest();

            await _userService.LogIn(logInDto);
            return Ok();
        }

        /// <summary>
        /// Register an account.
        /// </summary>
        /// <param name="registerDto">Account credentials.</param>
        /// <returns>Nothing.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad input.</response>
        [HttpPost]
        [AllowAnonymous]
        [Route("users/register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Register(UserRegisterDto registerDto)
        {
            if (!ModelState.IsValid) return BadRequest();

            await _userService.Register(registerDto);
            return Ok();
        }

        /// <summary>
        /// Log out.
        /// </summary>
        /// <returns>Nothing.</returns>
        /// <response code="200">Success.</response>
        /// <response code="401">Unauthorized user.</response>
        /// <response code="404">Unauthorized user.</response>
        [HttpPost]
        [Route("users/logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> LogOut([FromBody] object empty)
        {
            if (empty == null) return Unauthorized();

            await _userService.LogOut();
            return Ok();
        }
    }
}
