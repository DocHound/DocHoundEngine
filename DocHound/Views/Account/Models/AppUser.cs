using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Westwind.AspNetCore.Security;

namespace DocHound.Models
{
    public class AppUser : AppUserBase
    {
        public AppUser(ClaimsPrincipal user) : base(user)
        {
        }

        public Guid UserId
        {
            get
            {
                Guid userId = Guid.Empty;

                var strId = GetClaim("UserId");
                if (!string.IsNullOrEmpty(strId))
                    Guid.TryParse(strId, out userId);

                return userId;
            }
        }

        public string Username => GetClaim("Username");
        public string Email => GetClaim("Email");


        /// <summary>
        /// Comma delimited list of claims
        /// </summary>
        public string Roles => GetClaim("Roles");

        public bool IsAdmin => HasRole(RoleNames.Administrators);
        public bool IsOwner => HasRole(RoleNames.Owner);
        public bool IsContributor => HasRole(RoleNames.Contributor);

        public static ClaimsIdentity GetClaimsIdentityFromUser(User user)
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim("Email", user.Email));
            identity.AddClaim(new Claim("Username", user.UserDisplayName));
            identity.AddClaim(new Claim("UserId", user.Id.ToString()));

            if (user.IsAdmin)
                identity.AddClaim(new Claim(ClaimTypes.Role, RoleNames.Administrators));

            if (user.CurrentRepository.UserType == RepositoryUserType.Owner)
                identity.AddClaim(new Claim(ClaimTypes.Role, RoleNames.Owner));

            if (user.CurrentRepository.UserType == RepositoryUserType.Contributor || user.CurrentRepository.UserType == RepositoryUserType.Owner)
                identity.AddClaim(new Claim(ClaimTypes.Role, RoleNames.Contributor));

            if (!string.IsNullOrEmpty(user.CurrentRepository.Roles))
                identity.AddClaim(new Claim("Roles", user.CurrentRepository.Roles));

            return identity;
        }


        /// <summary>
        /// Determine whether a role name is part of this user's role for the active repository
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public bool IsUserInRole(string roleName)
        {
            if (string.IsNullOrEmpty(Roles))
                return false;

            return ("," + Roles + ",").Contains("," + roleName + ",", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Checks whether the user is in one or more roles
        /// </summary>
        /// <param name="roleNames">Comma delimited list of roles to check in user roles</param>
        /// <returns></returns>
        public bool IsUserInRoles(string roleNames)
        {
            if (string.IsNullOrEmpty(Roles))
                return false;

            var roles = "," + Roles + ",";

            var tokens = roleNames.Split(',');
            foreach (var roleName in tokens)
            {
                if (roles.Contains("," + roleName.Trim() + ",", StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }

            return false;
        }
    }


    public static class ClaimsPrincipalExtensions
    {
        public static AppUser GetAppUser(this ClaimsPrincipal user)
        {
            return new AppUser(user);
        }

    }

    public struct RoleNames
    {

        public const string Administrators = "Administrators";
        public const string Owner = "Owner";
        public const string Contributor = "Contributor";

    }
}