using System;
using System.Linq;
using System.Threading.Tasks;
using Likya.Core.EntityFramework;
using Likya.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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

            string[] roles = new string[] { "Owner", "Administrator", "Manager", "Editor", "Buyer", "Business", "Seller", "Subscriber" };

            // Add Roles
            if (context.Roles.Count() == 0)
            {
                foreach (string role in roles)
                {
                    var roleStore = new RoleStore<IdentityRole>(context);

                    if (!context.Roles.Any(r => r.Name == role))
                    {
                        roleStore.CreateAsync(new IdentityRole(role));
                    }
                }
            }

            // Add User
            var userStore = new UserStore<AppUser>(context);
            var user = new AppUser
            {
                FirstName = "Admin",
                LastName = "Admin",
                Email = "admin@hotmail.com",
                NormalizedEmail = "ADMIN@HOTMAIL.COM",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                PhoneNumber = "+111111111111",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            if (!context.Users.Any(m => m.UserName == user.UserName))
            {
                var password = new PasswordHasher<AppUser>();
                var hashed = password.HashPassword(user, "secret");
                user.PasswordHash = hashed;

                
                var createUserResult = userStore.CreateAsync(user);
            }

            // https://stackoverflow.com/questions/34343599/how-to-seed-users-and-roles-with-code-first-migration-using-identity-asp-net-cor
        }
    }
}
