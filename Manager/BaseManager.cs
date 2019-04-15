using AutoMapper;
using Infrastructure.Entity.User;
using Infrastructure.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MRMongoTools.Extensions.Identity.Interface;
using MRMongoTools.Extensions.Identity.Manager;

namespace Manager
{
    public abstract class BaseManager : MRAuthedManager
    {
        protected readonly IMapper _mapper;
        protected readonly IdentityUserManager _userManager;
        protected readonly IMRUserStore<UserEntity> _userStore;
        protected readonly ILogger _logger;

        protected bool _isUser => _isInRole(UserRoles.USER.ToString());
        protected bool _isAdmin => _isInRole(UserRoles.ADMIN.ToString());

        public BaseManager(ILoggerFactory _loggerFactory, IHttpContextAccessor httpContextAccessor, IMapper mapper, IdentityUserManager identityUserManager, IMRUserStore<UserEntity> mrUserStore) : base(httpContextAccessor)
        {
            _logger = _loggerFactory.CreateLogger(this.GetType());
            _mapper = mapper;
            _userManager = identityUserManager;
            _userStore = mrUserStore;
        }
    }
}
