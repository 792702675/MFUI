using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;
using Abp.Application.Services.Dto;
using MF.Users;
using MF.Users.Dto;

namespace MF.Tests
{
    public class StringExtensions_Tests 
    {

        [Fact]
        public void TrimStart_Test()
        {
            "".TrimStart("123").ShouldBe("");
            "123".TrimStart("123").ShouldBe("");
            "123/123121".TrimStart("123").ShouldBe("/123121");
            "123123121".TrimStart("123").ShouldBe("121");
        }

    }
}
