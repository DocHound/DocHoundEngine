using System;
using System.Threading.Tasks;
using DocHound.Models;
using NUnit.Framework;

namespace Tests
{
    public class UserBusinessTests
    {
        private const string STR_ConnectionString = "server=.;database=kavadocs;integrated security=true";
        private const string STR_RespositoryId = "66666666-6666-6666-AD11-DAE7FB1566CB";
        private const string STR_UserName = "rstrahl@west-wind.com";

        [SetUp]
        public void Setup()
        {
            
        }

        [Test]

        public async Task UserTest()
        {
            var userBusiness = new UserBusiness()
            {
                ConnectionString = STR_ConnectionString,
            };

            var user = await userBusiness.GetUserByEmail(STR_UserName,"docs");
            Assert.IsNotNull(user);

            Console.WriteLine(user.UserDisplayName);

        }

        [Test]
        public async Task UserRolesTest()
        {
            var userBusiness = new UserBusiness()
            {
                ConnectionString = STR_ConnectionString
            };

            var user = await userBusiness.GetUserByEmail(STR_UserName,"docs");

            string roles = await userBusiness.GetRolesAsync(user.Id, Guid.Parse(STR_RespositoryId));
            Assert.IsNotNull(roles);

            Console.WriteLine(roles);
        }
    }
}