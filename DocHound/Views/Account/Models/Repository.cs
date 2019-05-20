using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;

namespace DocHound.Models
{
    public class Repository
    {
        public Guid Id { get; set; }

        /// <summary>
        /// The domain Prefix and display key for this repository
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Prefix { get; set; }

        [StringLength(100)]
        public string Domain { get; set; } = "kavadocs.com";

        /// <summary>
        /// The display name of this repository
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public bool IsActive { get; set; }

        public RepositoryUserType UserType { get; set; } = RepositoryUserType.Owner;

        public string Roles { get; set; }

        public bool IsEmpty()
        {
            if (string.IsNullOrEmpty(Title))
                return true;

            return false;
        }


        /// <summary>
        /// Gets 
        /// </summary>
        /// <returns></returns>
        public string[] GetRoles()
        {
            if (Roles == null)
                return new string[0];

            return Roles.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Returns whether a given role exists on this users repository
        /// settings for this user
        /// </summary>
        /// <param name="role"></param>
        /// <returns>null if there are no roles. True or false otherwise</returns>
        public bool? IsInRole(string role)
        {
            var roles = GetRoles();

            if (roles.Length < 1)
                return null;   // should this be true?

            return roles.Any(r => r.Equals(role, StringComparison.InvariantCultureIgnoreCase));
        }

    }

    public enum RepositoryUserType
    {
        None = 0,
        User = 1,
        Contributor = 2,
        Owner = 4
    }
}