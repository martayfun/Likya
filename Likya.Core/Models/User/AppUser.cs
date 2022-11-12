using Microsoft.AspNetCore.Identity;

namespace Likya.Core.Models.User
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
