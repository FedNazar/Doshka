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

using Microsoft.AspNetCore.Identity;

namespace Doshka.Infrastructure.Repositories
{
    public interface IUserRepository : IRepository<IdentityUser, string>
    {
        public Task<IdentityResult> AddAsync(IdentityUser entity, string password);

        public Task<IdentityUser?> GetByEmailAsync(string email);
        public Task<IdentityUser?> GetByUserNameAsync(string userName);

        public Task<IdentityResult> AddToRoleAsync(IdentityUser entity, string role);
        public Task<IdentityResult> AddRoleAsync(string role);
        public Task<bool> CheckRoleAsync(IdentityUser user, string role);
        public Task<bool> RoleExistsAsync(string role);

        public Task<IdentityUser?> GetCurrentUserAsync(
            System.Security.Claims.ClaimsPrincipal user);

        public Task<SignInResult> LogInAsync(string email, string password);
        public Task LogOutAsync();
    }
}
