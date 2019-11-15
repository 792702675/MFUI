using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace MF.Authorization
{
    public class MFAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            //context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
            //context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
            //context.CreatePermission(PermissionNames.Pages_Roles, L("角色"));


            var pages = context.GetPermissionOrNull(PermissionNames.Pages) ?? context.CreatePermission(PermissionNames.Pages, L("全部"));

            var system = pages.CreateChildPermission(PermissionNames.Pages_Administration, L("系统"));
            system.CreateChildPermission(PermissionNames.Pages_Administration_Settings, L("系统配置项"));
            system.CreateChildPermission(PermissionNames.Pages_SystemEntrance, L("系统入口"));
            system.CreateChildPermission(PermissionNames.Pages_Administration_Menus, L("菜单"));
            system.CreateChildPermission(PermissionNames.Pages_Administration_AuditLogs, L("审计日志"));
            system.CreateChildPermission(PermissionNames.Pages_NoticeAnnouncementMange, L("公告通知"));

            var userAbout = pages.CreateChildPermission(PermissionNames.Pages_AboutUser, L("用户管理"));
            var organizationUnits = userAbout.CreateChildPermission(PermissionNames.Pages_Administration_OrganizationUnits, L("OrganizationUnits"));
            organizationUnits.CreateChildPermission(PermissionNames.Pages_Administration_OrganizationUnits_ManageOrganizationTree, L("ManagingOrganizationTree"));
            organizationUnits.CreateChildPermission(PermissionNames.Pages_Administration_OrganizationUnits_ManageMembers, L("ManagingMembers"));
            organizationUnits.CreateChildPermission(PermissionNames.Pages_Administration_OrganizationUnits_Lookup, L("查看组织机构"));

            userAbout.CreateChildPermission(PermissionNames.Pages_Roles, L("角色"));

            var users = userAbout.CreateChildPermission(PermissionNames.Pages_Administration_Users, L("用户"));
            users.CreateChildPermission(PermissionNames.Pages_Administration_Users_Create, L("新建"));
            users.CreateChildPermission(PermissionNames.Pages_Administration_Users_Edit, L("编辑"));
            users.CreateChildPermission(PermissionNames.Pages_Administration_Users_ChangeUserPassword, L("修改密码"));
            users.CreateChildPermission(PermissionNames.Pages_Administration_Users_Delete, L("删除"));
            users.CreateChildPermission(PermissionNames.Pages_Administration_Users_ChangePermissions, L("修改权限"));
            users.CreateChildPermission(PermissionNames.Pages_Administration_Users_Unlock, L("解锁"));
            users.CreateChildPermission(PermissionNames.Pages_Administration_Users_Active, L("启用禁用"));
            users.CreateChildPermission(PermissionNames.Pages_Administration_Users_Lookup, L("查看用户"));
            userAbout.CreateChildPermission(PermissionNames.Pages_ClassGroupMange, L("班组管理"));


            var data = pages.CreateChildPermission(PermissionNames.Pages_Data, L("数据管理"));
            data.CreateChildPermission(PermissionNames.Pages_TagManage, L("标签管理"));
            data.CreateChildPermission(PermissionNames.Oss_Aliyun_Manage, L("资源管理"));
            data.CreateChildPermission(PermissionNames.Pages_SkillManage, L("技能管理"));
            data.CreateChildPermission(PermissionNames.Pages_QuestLevelManage, L("谜题等级管理"));
            data.CreateChildPermission(PermissionNames.Pages_MicroworldThemeManage, L("微型世界主题"));
            data.CreateChildPermission(PermissionNames.Pages_CoursewareManage, L("学习资料"));
            var collection = data.CreateChildPermission(PermissionNames.Pages_Collection, L("收藏品管理"));
            collection.CreateChildPermission(PermissionNames.Pages_CollectionManage, L("收藏品基础配置"));
            collection.CreateChildPermission(PermissionNames.Pages_HumanBodyCollectionManage, L("人形收藏品管理"));
            collection.CreateChildPermission(PermissionNames.Pages_BadgeCollectionManage, L("徽章收藏品管理"));
            collection.CreateChildPermission(PermissionNames.Pages_AvatarCollectionManage, L("头像品基础管理"));
            data.CreateChildPermission(PermissionNames.Pages_QuizManage, L("题库管理"));


            var world = pages.CreateChildPermission(PermissionNames.Pages_World, L("世界运营"));
            world.CreateChildPermission(PermissionNames.Pages_MicroworldMange, L("微型世界"));
            world.CreateChildPermission(PermissionNames.Pages_AreaMange, L("区域"));
            world.CreateChildPermission(PermissionNames.Pages_QuestMange, L("谜题"));
            world.CreateChildPermission(PermissionNames.Pages_QuestOperationMange, L("谜题运营"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, MFConsts.LocalizationSourceName);
        }
    }
}
