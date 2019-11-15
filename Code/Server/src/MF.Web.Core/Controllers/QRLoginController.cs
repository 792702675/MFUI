using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using Abp.UI;
using MF.Authentication.External;
using MF.Authentication.JwtBearer;
using MF.Authorization;
using MF.Authorization.Users;
using MF.Models.TokenAuth;
using MF.MultiTenancy;
using System.ComponentModel.DataAnnotations;
using Abp.Web.Models;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Runtime.Caching;
using Microsoft.AspNetCore.SignalR;
using MF.QRLogin;

namespace MF.Controllers
{
    [Route("api/[controller]/[action]")]
    public class QRLoginController : MFControllerBase
    {
        private readonly ICacheManager _cacheManager;
        private readonly UserManager _userManager;
        private readonly IHubContext<QRLoginHub> _qrLoginHub;
        private readonly TokenAuthConfiguration _configuration;
        private readonly UserClaimsPrincipalFactory _userClaimsPrincipalFactory;
        public QRLoginController(
            UserManager userManager,
            ICacheManager cacheManager,
            IHubContext<QRLoginHub> qrLoginHub,
            TokenAuthConfiguration configuration,
           UserClaimsPrincipalFactory userClaimsPrincipalFactory)
        {
            _userManager = userManager;
            _cacheManager = cacheManager;
            _qrLoginHub = qrLoginHub;
            _configuration = configuration;
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        }

        [HttpPost]
        public async Task<bool> ScanQRCode(QRLoginInput input)
        {
            if (!AbpSession.UserId.HasValue)
            {
                throw new Abp.UI.UserFriendlyException("请先在手机上登录");
            }
            var findCode = _cacheManager.GetCache("QRLoginHub").GetOrDefault<string, QRCodeInfo>(input.ConnectionId);
            if (findCode == null)
            {
                throw new Abp.UI.UserFriendlyException("没有找到会话");
            }
            if (findCode.Token != input.Token)
            {
                throw new Abp.UI.UserFriendlyException("参数验证错误");
            }
            if (!findCode.IsValid())
            {
                throw new Abp.UI.UserFriendlyException("二维码已过期");
            }

            await _qrLoginHub.Clients.Client(input.ConnectionId).SendAsync("scanQRCode");
            return true;
        }

        [HttpPost]
        [AbpMvcAuthorize]
        public async Task<bool> ConfirmLogin(QRLoginInput input)
        {
            var findCode = _cacheManager.GetCache("QRLoginHub").GetOrDefault<string, QRCodeInfo>(input.ConnectionId);
            if (findCode == null)
            {
                throw new Abp.UI.UserFriendlyException("没有找到会话");
            }
            if (findCode.Token != input.Token)
            {
                throw new Abp.UI.UserFriendlyException("参数验证错误");
            }
            if (!findCode.IsValid())
            {
                throw new Abp.UI.UserFriendlyException("二维码已过期");
            }

            string token = Guid.NewGuid().ToString() + Guid.NewGuid().ToString();
            _cacheManager.GetCache("QRLoginHub").Remove(input.ConnectionId);
            _cacheManager.GetCache("QRLoginToken").Set(token, AbpSession.UserId.Value);
            await _qrLoginHub.Clients.Client(input.ConnectionId).SendAsync("confirmLogin", token);
            return true;
        }

        [HttpPost]
        public async Task<AuthenticateResultModel> Login(string token)
        {
            long userId = _cacheManager.GetCache("QRLoginToken").GetOrDefault<string, long>(token);
            if (userId == default)
            {
                throw new Abp.UI.UserFriendlyException("验证失败");
            }
            var user = await _userManager.GetUserByIdAsync(userId);

            var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
            var accessToken = CreateAccessToken(CreateJwtClaims(principal.Claims.ToList()));

            return new AuthenticateResultModel
            {
                AccessToken = accessToken,
                EncryptedAccessToken = GetEncrpyedAccessToken(accessToken),
                ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds,
                UserId = user.Id
            };
        }


        private string CreateAccessToken(IEnumerable<Claim> claims, TimeSpan? expiration = null)
        {
            var now = DateTime.UtcNow;

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(expiration ?? _configuration.Expiration),
                signingCredentials: _configuration.SigningCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        private static List<Claim> CreateJwtClaims(List<Claim> claims)
        {
            var nameIdClaim = claims.First(c => c.Type == ClaimTypes.NameIdentifier);

            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            });

            return claims;
        }

        private string GetEncrpyedAccessToken(string accessToken)
        {
            return SimpleStringCipher.Instance.Encrypt(accessToken, AppConsts.DefaultPassPhrase);
        }


        public class QRLoginInput
        {
            [Required]
            public string ConnectionId { get; set; }
            [Required]
            public Guid Token { get; set; }
        }
    }
}
