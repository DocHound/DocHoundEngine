using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DocHound.Interfaces;
using Newtonsoft.Json.Linq;

namespace DocHound.Classes
{
    public static class SqlDataAccess
    {
        public static bool CanUseSql => !string.IsNullOrEmpty(GetConnectionString());

        public static string GetConnectionString()
        {
            var connectionString = SettingsHelper.GetSetting<string>(SettingsEnum.SqlConnectionString);
            return connectionString;
        }

        public static async Task<dynamic> GetSqlRepositorySettingsDynamic(string prefix)
        {
            var settings = await GetSqlRepositorySettings(prefix);
            if (string.IsNullOrEmpty(settings)) return null;
            try
            {
                return JObject.Parse(settings);
            }
            catch
            {
                return null;
            }
        }

        public static async Task<string> GetSqlRepositorySettings(string prefix)
        {
            if (CanUseSql)
                return await GetRepositorySettings(prefix);
            return string.Empty;
        }

        public static async Task<string> GetRepositorySettings(string prefix, string defaultPrefix = "docs")
        {
            if (string.IsNullOrEmpty(prefix)) prefix = defaultPrefix;

            using (var connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                    using (var command = new SqlCommand("SELECT Settings FROM Repositories WHERE Prefix = @Prefix"))
                    {
                        command.Connection = connection;
                        command.Parameters.Add(new SqlParameter("@Prefix", prefix.Trim().ToLower()));
                        return await command.ExecuteScalarAsync() as string;
                    }

                return null;
            }
        }

        public static async Task<string> GetRepositoryLocalTableOfContents(string prefix, string defaultPrefix = "docs")
        {
            if (string.IsNullOrEmpty(prefix)) prefix = defaultPrefix;

            using (var connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                    using (var command = new SqlCommand("SELECT TableOfContents FROM Repositories WHERE Prefix = @Prefix"))
                    {
                        command.Connection = connection;
                        command.Parameters.Add(new SqlParameter("@Prefix", prefix.Trim().ToLower()));
                        return await command.ExecuteScalarAsync() as string;
                    }

                return null;
            }
        }

        public static async Task<string> GetRepositoryLocalCssOverrides(string prefix, string defaultPrefix = "docs")
        {
            if (string.IsNullOrEmpty(prefix)) prefix = defaultPrefix;

            using (var connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                    using (var command = new SqlCommand("SELECT CssOverrides FROM Repositories WHERE Prefix = @Prefix"))
                    {
                        command.Connection = connection;
                        command.Parameters.Add(new SqlParameter("@Prefix", prefix.Trim().ToLower()));
                        return await command.ExecuteScalarAsync() as string;
                    }

                return null;
            }
        }
    }
}