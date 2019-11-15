using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using MF.Users.Dto;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using MF.Authorization.Users;
using MF.Configuration;

namespace MF.Users
{
    [AbpAuthorize]
    public class UserLoginAppService : MFAppServiceBase, IUserLoginAppService
    {
        private readonly IRepository<UserLoginAttempt, long> _userLoginAttemptRepository;
        private readonly IRepository<User, long> _userRepository;

        public UserLoginAppService(
            IRepository<UserLoginAttempt, long> userLoginAttemptRepository,
            IRepository<User, long> userRepository
            )
        {
            _userLoginAttemptRepository = userLoginAttemptRepository;
            _userRepository = userRepository;
        }

        [DisableAuditing]
        public async Task<PagedResultDto<UserLoginAttemptDto>> GetRecentUserLoginAttempts(GetUserLoginsInput input)
        {
            var userId = AbpSession.GetUserId();
            var query = _userLoginAttemptRepository.GetAll()
                .Where(n => n.CreationTime >= input.StartDate)
                .Where(n => n.CreationTime < input.EndDate)
                .Where(la => la.UserId == userId);

            var resultCount = await query.CountAsync();
            var results = await query
                .AsNoTracking()
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();

            return new PagedResultDto<UserLoginAttemptDto>(resultCount, results.MapTo<List<UserLoginAttemptDto>>());
        }

    }
}