/*
 * Doshka
 * API Tests
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
using Doshka.API.Services;
using Doshka.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Doshka.API.Tests
{
    public class UserServiceTests
    {
        private IUserService _userService;

        private Mock<IUserRepository> _userRepository;

        [SetUp]
        public void SetUp()
        {
            _userRepository = new Mock<IUserRepository>();
            _userService = new UserService(_userRepository.Object);
        }

        [Test]
        public void Login_CorrectDetails()
        {
            SignInResult mockResult = SignInResult.Success;

            _userRepository.Setup(x => x.LogInAsync(It.IsAny<string>(), 
                It.IsAny<string>())).ReturnsAsync(mockResult);

            UserLogInDto credentials = new() { UserName = "FedNazar", 
                Password = "SuperSecure-123" };

            Assert.DoesNotThrowAsync(() => _userService.LogIn(credentials));
        }

        [Test]
        public void Login_IncorrectDetails_Fail()
        {
            SignInResult mockResult = SignInResult.Failed;

            _userRepository.Setup(x => x.LogInAsync(It.IsAny<string>(),
                It.IsAny<string>())).ReturnsAsync(mockResult);

            UserLogInDto credentials = new()
            {
                UserName = "FedNazar",
                Password = "let_me_in_pls"
            };

            Assert.CatchAsync<UnauthorizedAccessException>(() => 
                _userService.LogIn(credentials));
        }

        [Test]
        public void Register_ValidCredentials()
        {
            IdentityResult mockResult = IdentityResult.Success;

            _userRepository.Setup(x => x.AddAsync(It.IsAny<IdentityUser>(),
                It.IsAny<string>())).ReturnsAsync(mockResult);
            _userRepository.Setup(x => x.GetByUserNameAsync(It.IsAny<string>())).
                ReturnsAsync((IdentityUser?)null);
            _userRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).
                ReturnsAsync((IdentityUser?)null);
            _userRepository.Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), 
                "Player")).ReturnsAsync(mockResult);

            UserRegisterDto credentials = new()
            {
                Email = "me@example.com",
                UserName = "Test",
                Password = "SuperSecure-123"
            };

            Assert.DoesNotThrowAsync(() => _userService.Register(credentials));
        }

        [Test]
        public void Register_InvalidEmail_Fail()
        {
            IdentityResult mockResult = IdentityResult.Success;

            _userRepository.Setup(x => x.AddAsync(It.IsAny<IdentityUser>(),
                It.IsAny<string>())).ReturnsAsync(mockResult);
            _userRepository.Setup(x => x.GetByUserNameAsync(It.IsAny<string>())).
                ReturnsAsync((IdentityUser?)null);
            _userRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).
                ReturnsAsync((IdentityUser?)null);
            _userRepository.Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(),
                "Player")).ReturnsAsync(mockResult);

            UserRegisterDto credentials = new()
            {
                Email = "example.com",
                UserName = "Test",
                Password = "SuperSecure-123"
            };

            Assert.CatchAsync<ArgumentException>(() => _userService.Register(credentials));
        }

        [Test]
        public void Register_ExistingEmail_Fail()
        {
            IdentityResult mockResult = IdentityResult.Success;

            _userRepository.Setup(x => x.AddAsync(It.IsAny<IdentityUser>(),
                It.IsAny<string>())).ReturnsAsync(mockResult);
            _userRepository.Setup(x => x.GetByUserNameAsync(It.IsAny<string>())).
                ReturnsAsync((IdentityUser?)null);
            _userRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).
                ReturnsAsync(new IdentityUser());
            _userRepository.Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(),
                "Player")).ReturnsAsync(mockResult);

            UserRegisterDto credentials = new()
            {
                Email = "me@example.com",
                UserName = "Test",
                Password = "SuperSecure-123"
            };

            Assert.CatchAsync<ArgumentException>(() => _userService.Register(credentials));
        }

        [Test]
        public void Register_ExistingUserName_Fail()
        {
            IdentityResult mockResult = IdentityResult.Success;

            _userRepository.Setup(x => x.AddAsync(It.IsAny<IdentityUser>(),
                It.IsAny<string>())).ReturnsAsync(mockResult);
            _userRepository.Setup(x => x.GetByUserNameAsync(It.IsAny<string>())).
                ReturnsAsync(new IdentityUser());
            _userRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).
                ReturnsAsync((IdentityUser?)null);
            _userRepository.Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(),
                "Player")).ReturnsAsync(mockResult);

            UserRegisterDto credentials = new()
            {
                Email = "me@example.com",
                UserName = "Test",
                Password = "SuperSecure-123"
            };

            Assert.CatchAsync<ArgumentException>(() => _userService.Register(credentials));
        }
    }
}
