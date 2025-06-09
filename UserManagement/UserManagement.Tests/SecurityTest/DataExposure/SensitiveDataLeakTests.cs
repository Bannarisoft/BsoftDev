using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Users.Queries.GetUsers;

namespace UserManagement.Tests.SecurityTest.DataExposure
{
    [TestClass]
    public class SensitiveDataLeakTests
    {
        [TestMethod]
        public void ShouldNotExposeSensitiveFieldsInDTO()
        {
            var dto = new UserByIdDTO
            {
                UserId = 1,
                UserName = "test",
                EmailId = "test@example.com"
                // PasswordHash = "hashed"
            };

            var props = dto.GetType().GetProperties();
            Assert.IsFalse(props.Any(p => p.Name.Contains("PasswordHash") || p.Name.Contains("Salt")));
        }
    }
}