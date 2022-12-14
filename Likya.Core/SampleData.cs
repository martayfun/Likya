using System.Linq;
using Likya.Core.EntityFramework;
using Likya.Core.Models.User;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Likya.Core
{
    public static class SampleData
    {
        public static void Initialize(IApplicationBuilder app)
        {
            var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetService<ApplicationContext>();
            context.Database.Migrate();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var adminRole = new IdentityRole("Administrator");
            if (!context.Roles.Any())
            {
                roleManager.CreateAsync(adminRole).GetAwaiter().GetResult();
            }

            if (!context.Users.Any(m => m.UserName == "administrator"))
            {
                var adminUser = new AppUser
                {
                    UserName = "administrator",
                    Email = "administrator@hotmail.com"
                };

                userManager.CreateAsync(adminUser, "123456a").GetAwaiter().GetResult();
                userManager.AddToRoleAsync(adminUser, adminRole.Name).GetAwaiter().GetResult();
            }

            // https://stackoverflow.com/questions/34343599/how-to-seed-users-and-roles-with-code-first-migration-using-identity-asp-net-cor
        }
    }
}
