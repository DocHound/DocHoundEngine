using System;
using System.ComponentModel.DataAnnotations;

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

        public bool IsEmpty()
        {
            if (string.IsNullOrEmpty(Title))
                return true;

            return false;
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