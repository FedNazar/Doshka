/*
 * Doshka
 * Infrastructure Layer
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

using Doshka.Infrastructure.DTOs;
using Doshka.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;

namespace Doshka.Infrastructure.Services
{
    public class UserSeederService : IUserSeederService
    {
        private IUserRepository _userRepository;

        public UserSeederService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task SeedRolesAsync()
        {
            string[] roles = { "Admin", "Player" };

            foreach (string role in roles)
            {
                if (await _userRepository.RoleExistsAsync(role)) continue;

                IdentityResult result = await _userRepository.AddRoleAsync(role);
                if (!result.Succeeded) throw new Exception(
                    $"Unable to seed role \"{role}\"");
            }
        }

        public async Task SeedUsersAsync()
        {
            UserSeedDto[]? users = JsonSerializer.Deserialize<UserSeedDto[]>(
                await File.ReadAllTextAsync("seed-users.json"));

            if (users == null) return;

            foreach (UserSeedDto userInfo in users)
            {
                if ((await _userRepository.GetByUserNameAsync(userInfo.UserName)) != null)
                    throw new ArgumentException("This username is already registered");

                IdentityUser user = new IdentityUser(userInfo.UserName);

                if (!userInfo.Email.Contains('@')) throw new ArgumentException(
                    "Invalid email address");
                if ((await _userRepository.GetByEmailAsync(userInfo.Email)) != null)
                    throw new ArgumentException("This email is already registered");

                user.Email = userInfo.Email;

                IdentityResult result = await _userRepository.AddAsync(
                    user, userInfo.Password);
                if (!result.Succeeded) throw new Exception(
                    $"Unable to seed user \"{userInfo.UserName}\"");

                if (!(await _userRepository.AddToRoleAsync(user, userInfo.Role)).Succeeded)
                    throw new ArgumentException($"Unable to assign the role \"{userInfo.Role}\"");
            }
        }
    }
}
