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
using Doshka.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Text;

namespace Doshka.API.Services
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task LogIn(UserLogInDto userDto)
        {
            SignInResult result = await _userRepository.LogInAsync(
                userDto.UserName, userDto.Password);

            if (!result.Succeeded) throw new UnauthorizedAccessException();
        }

        public async Task Register(UserRegisterDto userDto)
        {
            if ((await _userRepository.GetByUserNameAsync(userDto.UserName)) != null)
                throw new ArgumentException("This username is already registered");

            IdentityUser user = new IdentityUser(userDto.UserName);

            if (!userDto.Email.Contains('@')) throw new ArgumentException(
                "Invalid email address");
            if ((await _userRepository.GetByEmailAsync(userDto.Email)) != null)
                throw new ArgumentException("This email is already registered");

            user.Email = userDto.Email;

            IdentityResult result = await _userRepository.AddAsync(
                user, userDto.Password);

            if (!result.Succeeded)
            {
                StringBuilder exceptionStr = new StringBuilder("Unable to register\n");

                foreach (IdentityError error in result.Errors)
                {
                    exceptionStr.AppendLine(error.Description);
                }

                throw new ArgumentException(exceptionStr.ToString());
            }

            if (!(await _userRepository.AddToRoleAsync(user, "Player")).Succeeded)
                throw new ArgumentException("Unable to assign the role \"Player\"");
        }

        public async Task LogOut()
        {
            await _userRepository.LogOutAsync();
        }
    }
}
