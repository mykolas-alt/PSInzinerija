using System.Net.Http.Json;

using Backend.Data.ApplicationDbContext;
using Backend.Data.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BackendTests.Integration.Helpers
{
    public static class Utilities
    {
        public static readonly string TestEmail = "test@mailas.com";
        public static readonly string TestUserName = "linksmas";
        public static readonly string TestPassword = "Abcd1!";

        public static async Task CreateUser(IServiceProvider sp)
        {
            var userManager = sp.GetRequiredService<UserManager<User>>();
            var userStore = sp.GetRequiredService<IUserStore<User>>();
            var emailStore = (IUserEmailStore<User>)userStore;
            var email = TestEmail;
            var userName = TestUserName;
            var password = TestPassword;

            var user = new User();
            await userStore.SetUserNameAsync(user, userName, CancellationToken.None);
            await emailStore.SetEmailAsync(user, email, CancellationToken.None);
            var result = await userManager.CreateAsync(user, password);
        }

        public static async Task<HttpClient> CreateAuthenticatedClient(this CustomWebApplicationFactory<Program> webApplicationFactory)
        {
            var _factory = webApplicationFactory;
            var client = _factory.CreateClient();
            using var scope = _factory.Services.CreateScope();
            var sp = scope.ServiceProvider;
            await CreateUser(sp);

            var body1 = new
            {
                email = TestEmail,
                password = TestPassword
            };
            var resp1 = await client.PostAsJsonAsync("/login?useCookies=true&useSessionCookies=true", body1);

            return client;
        }

        public static void ResetUsersScoresTable(this CustomWebApplicationFactory<Program> webApplicationFactory)
        {
            using var scope = webApplicationFactory.Services.CreateScope();
            var dbcontext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbcontext.Users.RemoveRange(dbcontext.Users);
            dbcontext.SaveChanges();
        }

        public static void ResetHighScoresTable(this CustomWebApplicationFactory<Program> webApplicationFactory)
        {
            using var scope = webApplicationFactory.Services.CreateScope();
            var dbcontext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbcontext.HighScores.RemoveRange(dbcontext.HighScores);
            dbcontext.SaveChanges();
        }
    }
}