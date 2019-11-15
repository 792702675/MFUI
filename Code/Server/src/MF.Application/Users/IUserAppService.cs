using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using MF.Roles.Dto;
using MF.Users.Dto;

namespace MF.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedUserResultRequestDto, CreateUserDto, UpdateUserDto>
    {
        Task<ListResultDto<RoleListDto>> GetRoles();
        Task ChangeLanguage(ChangeUserLanguageDto input);


        Task<GetUserForEditOutput> GetUserForEdit(NullableIdDto<long> input);
        Task<GetUserPermissionsForEditOutput> GetUserPermissionsForEdit(EntityDto<long> input);
        Task ResetUserSpecificPermissions(EntityDto<long> input);
        Task UpdateUserPermissions(UpdateUserPermissionsInput input);
        Task BatchDelete(CommonDto.ArrayDto<long> input);
        Task ToggleActiveStatus(EntityDto<long> input);
        Task UnlockUser(EntityDto<long> input);
        Task BatchUnlockUser(CommonDto.ArrayDto<long> input);
        Task BatchActiveUser(BatchActiveUserInput input);
        Task UpdateCurrentUser(UpdateCurrentUserInput input);
    }
}
