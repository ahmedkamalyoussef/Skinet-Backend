
using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Skinet.Core.Entites;

namespace Skinet.API.Extensions
{
    public static class ClaimPrincipleExtentions
    {
        public static async Task<AppUser> GetUserByEmail(this UserManager<AppUser> userManager, ClaimsPrincipal userClaimes)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x =>
             x.Email == userClaimes.GetEmail()) ?? throw new AuthenticationException("User not found");
            return user;
        }
        public static async Task<AppUser> GetUserByEmailWithAddress(this UserManager<AppUser> userManager, ClaimsPrincipal userClaimes)
        {
            var user = await userManager.Users.Include(u=>u.Address).FirstOrDefaultAsync(x =>
             x.Email == userClaimes.GetEmail()) ?? throw new AuthenticationException("User not found");
            return user;
        }

        public static string GetEmail(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Email) ?? throw new AuthenticationException("Email not found");
        }
    }
}