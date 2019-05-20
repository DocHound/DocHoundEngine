using DocHound.Classes;
using DocHound.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace DocHound.Models
{
    public class UserBusiness
    {
        public string ErrorMessage { get; set; }

        public string ConnectionString { get; set; }

        public async Task<DocHound.Models.User> Authenticate(string username, string password, 
                                 string prefix, string domain="kavadocs.com")
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ErrorMessage = "Invalid username or password.";
                return null;
            }

            // assumes only no dupe email addresses
            var user = await GetUserByEmail(username, prefix, domain);
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
            string domainPrefix,
            string domain = "kavadocs.com")
        {
            string sql = @"select UR.UserType, U.Id, U.email, U.UserDisplayName, U.Company,
          U.IsActive, U.IsAdmin, U.Password, UR.UserType, 
          r.Title, r.Prefix, r.Domain, r.IsActive
        FROM Users as U
        LEFT JOIN UserRepositories as UR ON UR.UserId = U.Id
        LEFT JOIN Repositories as R on UR.RepositoryId = R.Id
        where Prefix=@prefix and Domain=@domain and email=@email and R.IsActive = 1 and U.IsActive =1";

            string conn = GetConnectionString();
            
            using (var connection = new SqlConnection(conn))
            {
                User user = null;

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


                        user = new User()
                        {
                            UserDisplayName = reader["UserDisplayName"] as string,
                            Email = reader["Email"] as string,
                            IsAdmin = (bool) reader["IsAdmin"],
                            Id = (Guid) reader["Id"],
                            Password = reader["Password"] as string
                        };

                        
                        var repo = user.CurrentRepository;
                        if (repo != null && repo.Id != Guid.Empty)
                        {
                            repo.Prefix = reader["Prefix"] as string;
                            repo.Title = reader["Title"] as string;
                            var userType = (int) reader["UserType"];
                            repo.UserType = (RepositoryUserType) userType;

                            
                            repo.Roles = await GetRolesAsync(user.Id, repo.Id);
                        }

                    }
                    
                    return user;
            }


            return null;
        }
        
        public async Task<string> GetRolesAsync(Guid userId, Guid repoId)
        {
            var sql = @"
SELECT R.name, R.id from UserRoles as UR	
	LEFT JOIN Roles as R on R.Id = UR.RoleId
    WHERE UR.RepositoryId = @repoId and UR.Userid = @userId";

            using (var connection = new SqlConnection(GetConnectionString()))
            {
                var reader = await ExecuteSqlReaderAsync(
                    connection, 
                    sql,
                    new SqlParameter("@repoId", repoId),
                    new SqlParameter("@userId", userId));

                if (reader == null || !reader.HasRows)
                    return null;

                StringBuilder sb = new StringBuilder(100);

                while (reader.Read())
                {
                    string role = reader["name"] as string;
                    if (!string.IsNullOrEmpty(role))
                        sb.Append(role + ",");
                }

                if (sb.Length > 0)
                    sb.Length--;

                return sb.ToString();
            }
        }


        private async Task<SqlDataReader> ExecuteSqlReaderAsync(SqlConnection connection, string sql,
            params SqlParameter[] parameters)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();

            using (var command = new SqlCommand(sql))
            {
                command.Connection = connection;
                if (parameters != null)
                    command.Parameters.AddRange(parameters);

                var reader = await command.ExecuteReaderAsync();
                return reader;
            }
        }

        private string GetConnectionString()
        {
            if (!string.IsNullOrEmpty(ConnectionString))
                return ConnectionString;

            return SqlDataAccess.GetConnectionString();
        }

    }
}