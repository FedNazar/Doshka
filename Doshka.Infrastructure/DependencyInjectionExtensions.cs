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

using Doshka.Infrastructure.Repositories;
using Doshka.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Doshka.Infrastructure
{
    public static class DependencyInjectionExtensions
    { 
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, string? dbConnectionStr)
        {
            services.AddDbContext<ApplicationDbContext>((servProvider, options) =>
                options.UseSqlServer(dbConnectionStr)
                .UseSeeding((ctx, _) =>
                {
                    IUserSeederService userSeederService = servProvider.
                        GetRequiredService<IUserSeederService>();

                    Task.Run(() => userSeederService.SeedRolesAsync()).GetAwaiter().GetResult();
                    Task.Run(() => userSeederService.SeedUsersAsync()).GetAwaiter().GetResult();
                })
                .UseAsyncSeeding(async (ctx, _, ct) => {
                    IUserSeederService userSeederService = servProvider.
                        GetRequiredService<IUserSeederService>();

                    await userSeederService.SeedRolesAsync();
                    await userSeederService.SeedUsersAsync();
                })
            );

            services
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPlayerService, PlayerService>();

            services.AddScoped<ILeaderboardEntryRepository, LeaderboardEntryRepository>();
            services.AddScoped<ILeaderboardRepository, LeaderboardRepository>();

            services.AddScoped<IUserSeederService, UserSeederService>();

            return services;
        }
    }
}
