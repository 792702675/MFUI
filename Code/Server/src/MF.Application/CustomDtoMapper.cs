using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.AutoMapper;
using AutoMapper;
using MF.AppEditions;
using MF.AppEditions.Dto;
using MF.Authorization.Roles;
using MF.Authorization.Users;
using MF.OSS;
using MF.Roles.Dto;
using MF.Sessions.Dto;
using MF.Users.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MF
{
    internal static class CustomDtoMapper
    {
        private static volatile bool _mappedBefore;
        private static readonly object SyncObj = new object();

        public static void CreateMappings(IMapperConfigurationExpression mapper)
        {
            lock (SyncObj)
            {
                if (_mappedBefore)
                {
                    return;
                }

                CreateMappingsInternal(mapper);

                _mappedBefore = true;
            }
        }

        private static void CreateMappingsInternal(IMapperConfigurationExpression mapper)
        {
            mapper.CreateMap<User, CreateUserDto>()
                .ForMember(dto => dto.Password, options => options.Ignore())
                .ReverseMap()
                .ForMember(user => user.Password, options => options.Ignore());
            mapper.CreateMap<User, UpdateUserDto>()
                .ForMember(dto => dto.Password, options => options.Ignore())
                .ReverseMap()
                .ForMember(user => user.Password, options => options.Ignore());

            // Role and permission
            mapper.CreateMap<Permission, string>().ConvertUsing(r => r.Name);
            mapper.CreateMap<RolePermissionSetting, string>().ConvertUsing(r => r.Name);

            mapper.CreateMap<CreateRoleDto, Role>().ForMember(x => x.Permissions, opt => opt.Ignore());
            mapper.CreateMap<RoleDto, Role>().ForMember(x => x.Permissions, opt => opt.Ignore());

            mapper.CreateMap<UserDto, User>();
            mapper.CreateMap<UserDto, User>().ForMember(x => x.Roles, opt => opt.Ignore());

            mapper.CreateMap<CreateUserDto, User>();
            mapper.CreateMap<CreateUserDto, User>().ForMember(x => x.Roles, opt => opt.Ignore());

            mapper.CreateMap<User, UserLoginInfoDto>().ForMember(x => x.Profile, opt => opt.MapFrom(n => $"/Profile/GetProfilePictureById/{n.ProfilePictureId}"));

            mapper.CreateMap<DateTime, string>().ConvertUsing(n => n.ToString("yyyy/MM/dd HH:mm"));
            mapper.CreateMap<DateTime?, string>().ConvertUsing(n => n.HasValue ? n.Value.ToString("yyyy/MM/dd HH:mm") : "");

            mapper.CreateMap<AppEdition, AppEditionDto>()
                .ForMember(x => x.AppType, opt => opt.MapFrom(n => n is IOSAppEdition ? "IOS" : n is AndroidAppEdition ? "Android" : ""))
                .ForMember(x => x.ItunesUrl, opt => opt.MapFrom(n => n is IOSAppEdition ? ((IOSAppEdition)n).ItunesUrl : ""));




            mapper.CreateMap<OssETagFileDto, OssETagFile>().ConvertUsing(n =>
            n == null ? new OssETagFile() : new OssETagFile { BucketName = n.BucketName, ETag = n.ETag });



        }
    }
}