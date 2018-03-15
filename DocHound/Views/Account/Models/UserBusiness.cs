using DocHound.Classes;
using DocHound.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DocHound.Models
{
    public class UserBusiness
    {
        public string ErrorMessage { get; set; }

        public async Task<DocHound.Models.User> Authenticate(string username, string password, 
                                 string prefix = "docs", string domain="kavadocs.com")
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ErrorMessage = "Invalid username or password.";
                return null;
            }

            // assumes only no dupe email addresses
            var user = await GetUserByEmail(username);
            if(user == null)
            {
                ErrorMessage = "Invalid username or password.";
                return null;
            }

            string passwordHash = User.HashPassword(password, user.Id.ToString());
            if (user.Password != passwordHash && user.Password != password)
            {
                ErrorMessage = "Invalid username or password.";
                return null;
            }

            return user;
        }


        public async Task<DocHound.Models.User> GetUserByEmail(string email, 
            string domainPrefix = "docs",
            string domain = "kavadocs.com")
        {
            string sql = @"select UR.UserType, U.Id, U.email, U.UserDisplayName, U.Company,
          U.IsActive, U.IsAdmin, U.Password, UR.UserType, 
          r.Title, r.Prefix, r.Domain, r.IsActive
        FROM Users as U
        LEFT JOIN UserRepositories as UR ON UR.UserId = U.Id
        LEFT JOIN Repositories as R on UR.RepositoryId = R.Id
        where Prefix=@prefix and Domain=@domain and email=@email and R.IsActive = 1 and U.IsActive =1";


            using (var connection = new SqlConnection(SqlDataAccess.GetConnectionString()))
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                    using (var command = new SqlCommand(sql))
                    {
                        command.Connection = connection;
                        command.Parameters.Add(new SqlParameter("@email", email));
                        command.Parameters.Add(new SqlParameter("@prefix", domainPrefix));
                        command.Parameters.Add(new SqlParameter("@domain", domain));

                        var reader = await command.ExecuteReaderAsync();
                        if (reader == null || !reader.HasRows)
                            return null;

                        reader.Read();


                        var user = new User()
                        {
                            UserDisplayName = reader["UserDisplayName"] as string,
                            Email = reader["Email"] as string,
                            IsAdmin = (bool) reader["IsAdmin"],
                            Id = (Guid) reader["Id"],
                            Password = reader["Password"] as string
                        };

                        var repo = user.CurrentRepository;
                        repo.Prefix = reader["Prefix"] as string;
                        repo.Title = reader["Title"] as string;
                        var userType = (int) reader["UserType"];
                        repo.UserType = (RepositoryUserType) userType;
                        return user;
                    }                

            }

            return null;
        }

    }
}