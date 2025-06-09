using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace UserManagement.Tests.SecurityTest.Authorization
{
    [TestClass]
    public class RoleBasedAccessTests
    {
        [TestMethod]
        public void Only_Admins_ShouldHaveAccess()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.Name, "user1"),
            new Claim(ClaimTypes.Role, "User") // Not Admin
        }, "mock"));

            var context = new DefaultHttpContext { User = user };

            var isAuthorized = context.User.IsInRole("Admin");

            Assert.IsFalse(isAuthorized);
        }
    }
}