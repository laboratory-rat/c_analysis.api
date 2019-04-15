using AutoMapper;
using Infrastructure.Entity.User;
using Infrastructure.Enums;
using Infrastructure.Interface.Manager;
using Infrastructure.Model.User;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MRIdentityClient;
using MRIdentityClient.Exception.Basic;
using MRIdentityClient.Exception.User;
using MRIdentityClient.Response;
using MRMongoTools.Extensions.Identity.Component;
using MRMongoTools.Extensions.Identity.Interface;
using MRMongoTools.Extensions.Identity.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager
{
    public class LoginManager : BaseManager, ILoginManager
    {
        protected readonly IdentityClient _identityClient;
        protected readonly MRTokenManager<UserEntity> _tokenManager;

        public LoginManager(ILoggerFactory _loggerFactory, IHttpContextAccessor httpContextAccessor, IMapper mapper,
            IdentityUserManager mrUserManager, IMRUserStore<UserEntity> mrUserStore, IdentityClient identityClient, MRTokenManager<UserEntity> tokenManager) : base(_loggerFactory, httpContextAccessor, mapper, mrUserManager, mrUserStore)
        {
            _identityClient = identityClient;
            _tokenManager = tokenManager;
        }

        /// <summary>
        /// Approve login from MR Identity
        /// </summary>
        /// <param name="token">Mr identity token</param>
        /// <returns></returns>
        public async Task<UserLoginResponse> Approve(string token)
        {
            var identityResponse = await _identityClient.Signup.ApproveLogin(token);
            if (!identityResponse.IsSuccess)
            {
                _logger.LogError(identityResponse.Error.Message);
                throw new LoginFailedException("Authorize faild");
            }

            var identityUser = identityResponse.Response;
            var exists = await _userStore.FindByEmailAsync(identityUser.Email, new System.Threading.CancellationToken());
            var roles = new List<string>();

            if (exists == null)
            {
                exists = new UserEntity
                {
                    Email = identityUser.Email,
                    NormalizedEmail = identityUser.Email.ToUpperInvariant(),
                    Sex = MRMongoTools.Extensions.Identity.Enum.UserSex.UNDEFINED,
                    UserName = identityUser.Email,
                    NormalizedUserName = identityUser.Email.ToUpperInvariant(),
                    Tels = new List<MRUserTel>(),
                    Tokens = new List<MRUserToken>(),
                    Roles = new List<MRUserRole>(),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    LastName = identityUser.LastName,
                    FirstName = identityUser.FirstName,
                };

                await _userManager.CreateAsync(exists);
                await _userManager.AddToRoleAsync(exists, UserRoles.USER.ToString());
                roles = new List<string>() { UserRoles.USER.ToString() };
            }
            else
            {
                exists.FirstName = identityUser.FirstName;
                exists.LastName = identityUser.LastName;

                await _userManager.UpdateAsync(exists);
                roles = (await _userManager.GetRolesAsync(exists))?.ToList();
            }

            await _userManager.AddLoginAsync(exists, new Microsoft.AspNetCore.Identity.UserLoginInfo("MR_IDENTITY", token, "MR Identity"));
            await _userManager.AddClaimsAsync(exists, _tokenManager.GetClaims(exists, roles));

            UserLoginResponse result = _mapper.Map<UserLoginResponse>(exists);
            result.Roles = roles;

            var tokenTuple = _tokenManager.Generate(exists, roles);
            result.Token = new UserLoginResponseToken
            {
                Expire = tokenTuple.Item2,
                Token = tokenTuple.Item1
            };

            return result;
        }

        /// <summary>
        /// Check is user login in system
        /// </summary>
        /// <returns><see cref="ApiOkResult"/></returns>
        public ApiOkResult PingLogin()
        {
            if (!string.IsNullOrWhiteSpace(_currentUserId))
                return new ApiOkResult();

            return new ApiOkResult(false);
        }

        /// <summary>
        /// Logout user
        /// </summary>
        /// <returns><see cref="ApiOkResult"/></returns>
        public async Task<ApiOkResult> Logout()
        {
            if (!string.IsNullOrWhiteSpace(_currentUserId))
                return new ApiOkResult();

            var user = await _userManager.FindByIdAsync(_currentUserId);
            var roles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveClaimsAsync(user, _tokenManager.GetClaims(user, roles));
            return new ApiOkResult();
        }

    }
}
