using System.Collections.Generic;
using System.Linq;
using Abp.Application.Navigation;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Localization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MF.Configuration;
using MF.EntityFrameworkCore;
using MF.Web;

namespace MF.Menus
{
    /// <summary>
    /// 菜单导航管理
    /// </summary>
    public class DBNavigationProvider : NavigationProvider, ISingletonDependency
    {

        private static INavigationProviderContext _context;

        /// <summary>
        /// 菜单组名称
        /// </summary>
        public const string MenuGroupName = "DB";
        private MFDbContext CreateDbContext()
        {
            var builder = new DbContextOptionsBuilder<MFDbContext>();
            var hostingEnvironment= IocManager.Instance.Resolve<IWebHostEnvironment>();
            var configuration = AppConfigurations.Get(hostingEnvironment.ContentRootPath);

            builder.UseSqlServer(configuration.GetConnectionString(MFConsts.ConnectionStringName));

            return new MFDbContext(builder.Options);
        }
        /// <summary>
        /// 构建导航菜单
        /// </summary>
        /// <param name="context"></param>
        public override void SetNavigation(INavigationProviderContext context)
        {
            if (context != null)
            {
                _context = context;
            }
            else
            {
                context = _context;
            }
            var menu = new MenuDefinition(MenuGroupName, new FixedLocalizableString("主菜单"));
            context.Manager.Menus[MenuGroupName] = menu;
            var dbContext = CreateDbContext();
            //var _menuRepository = IocManager.Instance.Resolve<IRepository<Menu>>();
            var menuItemList = dbContext.Menus //_menuRepository.GetAll()
                .Where(m => m.IsVisible)
                .ToList();
            var allMenus = new Dictionary<int, MenuItemDefinition>();
            foreach (var item in menuItemList)
            {
                var itemDefinition = new MenuItemDefinition(
                    item.Id.ToString(), L(item.DisplayName),
                    url: item.Url, icon: item.Icon, requiredPermissionName: item.RequiredPermissionName,
                    order: item.Order, isEnabled: item.IsEnabled, target: item.Target);
                allMenus.Add(item.Id, itemDefinition);
            }
            foreach (var item in menuItemList)
            {
                if (item.ParentId == 0)
                {
                    menu.AddItem(allMenus[item.Id]);
                }
                else
                {
                    var parent = allMenus[item.ParentId];
                    parent.AddItem(allMenus[item.Id]);
                }
            }
        }

        private static ILocalizableString L(string name)
        {
            return new FixedLocalizableString(name);
        }
    }
}