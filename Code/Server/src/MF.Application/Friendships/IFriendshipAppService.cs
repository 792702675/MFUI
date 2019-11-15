using System.Threading.Tasks;
using Abp.Application.Services;
using MF.Friendships.Dto;
using Abp.Application.Services.Dto;
using System.Collections.Generic;
using MF.CommonDto;

namespace MF.Friendships
{
    public interface IFriendshipAppService : IApplicationService
    {
        Task<FriendDto> CreateFriendshipRequest(CreateFriendshipRequestInput input);

        Task<FriendDto> CreateFriendshipRequestByUserName(CreateFriendshipRequestByUserNameInput input);

        Task BlockUser(BlockUserInput input);

        Task UnblockUser(UnblockUserInput input);

        Task AcceptFriendshipRequest(AcceptFriendshipRequestInput input);

        Task<List<FriendDto>> BatchCreateFriendshipRequestAsync(List<CreateFriendshipRequestInput> input);

        Task<PagedResultDto<FirendshipUserDto>> GetCreateFriendshipUserList(PagedAndFilteredInputDto input);
    }
}
