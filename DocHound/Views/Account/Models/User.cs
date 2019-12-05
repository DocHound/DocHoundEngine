using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Westwind.Utilities;

namespace DocHound.Models
{
    [DebuggerDisplay("{UserDisplayName}")]
    public class User
    {
        public User()
        {
            Id = Guid.NewGuid();            
            CurrentRepository = new Repository();
        }

        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string UserDisplayName { get; set; }

        [StringLength(150)]
        [Required]
        public string Email { get; set; }      

        public bool IsAdmin { get; set; }                      

        [JsonIgnore]
        [Required]
        [StringLength(80)]
        public string Password
        {
            get { return _password; }
            set => _password = HashPassword(value, Id.ToString());
        }
        [XmlIgnore]
        private string _password;
        
        //public List<RepositoryUser> Repositories { get; set; }

        public Repository CurrentRepository { get; set; }

        #region Utilities and Helpers
        const string HashPostFix = "|~|";

        /// <summary>
        /// Returns an hashed and salted password.
        /// 
        /// Encoded Passwords end in || to indicate that they are 
        /// encoded so that bus objects can validate values.
        /// </summary>
        /// <param name="password">The password to convert</param>
        /// <param name="uniqueSalt">
        /// Unique per instance salt - use user id</param>
        /// <param name="appSalt">Salt to apply to the password</param>
        /// <returns>Hashed password. If password passed is already a hash
        /// the existing hash is returned
        /// </returns>
        public static string HashPassword(string password, string uniqueSalt,
            string appSalt = "#5518-21%5#36@")
        {
            // don't allow empty password
            if (string.IsNullOrEmpty(password))
                return string.Empty;

            // already encoded
            if (password.EndsWith(HashPostFix))
                return password;

            string saltedPassword = uniqueSalt + password + appSalt;

            // pre-hash
            var sha = new SHA1CryptoServiceProvider();
            byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(saltedPassword));

            // hash again
            var sha2 = new SHA256CryptoServiceProvider();
            hash = sha2.ComputeHash(hash);

            return StringUtils.BinaryToBinHex(hash) + HashPostFix;
        }

        #endregion
    }



}
