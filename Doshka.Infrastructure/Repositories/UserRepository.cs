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
    public class UserRepository : IUserRepository
    {
        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private SignInManager<IdentityUser> _signInManager;

        public UserRepository(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityUser?> GetByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<IdentityUser?> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IdentityUser?> GetByUserNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task AddAsync(IdentityUser entity)
        {
            await _userManager.CreateAsync(entity);
        }

        public async Task<IdentityResult> AddAsync(IdentityUser entity, string password)
        {
            return await _userManager.CreateAsync(entity, password);
        }

        public async Task DeleteAsync(IdentityUser entity)
        {
            await _userManager.DeleteAsync(entity);
        }

        public async Task UpdateAsync(IdentityUser entity)
        {
            await _userManager.UpdateAsync(entity);
        }

        public async Task<IdentityResult> AddToRoleAsync(IdentityUser entity, 
            string role)
        {
            return await _userManager.AddToRoleAsync(entity, role);
        }

        public async Task<IdentityResult> AddRoleAsync(string role)
        {
            return await _roleManager.CreateAsync(new IdentityRole(role));
        }

        public async Task<IdentityUser?> GetCurrentUserAsync(
            System.Security.Claims.ClaimsPrincipal user)
        {
            return await _userManager.GetUserAsync(user);
        }

        public async Task<bool> CheckRoleAsync(IdentityUser user, string role)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<bool> RoleExistsAsync(string role)
        {
            return await _roleManager.RoleExistsAsync(role);
        }

        public async Task<SignInResult> LogInAsync(string email, string password)
        {
            return await _signInManager.PasswordSignInAsync(
                email, password, isPersistent: true, lockoutOnFailure: true);
        }

        public async Task LogOutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
