using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Navigation;
using Abp.Application.Services;
using MF.Menus.Dto;

namespace MF.Menus
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMenuAppService : IApplicationService
    {
        /// <summary>
        /// 创建用户自定义菜单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<MenuDto> CreateCustomMenu(CreateMenuInput input);

        /// <summary>
        /// 创建系统菜单，开发人员使用
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<MenuDto> CreateSystemMenu(CreateMenuInput input);

        /// <summary>
        /// 更新菜单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<MenuDto> UpdateMenu(UpdateMenuInput input);

        /// <summary>
        /// 移动菜单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<MenuDto> MoveMenu(MoveMenuInput input);

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task DeleteMenu(DeleteMenuInput input);

        /// <summary>
        /// 获取菜单及权限
        /// </summary>
        /// <returns></returns>
        Task<IList<UserMenuItem>> GetUserMenus();

        /// <summary>
        /// 获取菜单及权限
        /// </summary>
        /// <returns></returns>
        Task<IList<MenuDto>> GetAllMenus();
    }
}