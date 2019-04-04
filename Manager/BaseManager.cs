using AutoMapper;
using Infrastructure.Enums;
using Microsoft.AspNetCore.Http;
using MRMongoTools.Extensions.Identity.Interface;
using MRMongoTools.Extensions.Identity.Manager;

namespace Manager
{
    public abstract class BaseManager : MRAuthedManager
    {
        protected readonly IMapper _mapper;
        protected readonly MRUserManager _userManager;
        protected readonly IMRUserStore _userStore;

        protected bool _isUser => _isInRole(UserRoles.USER.ToString());
        protected bool _isAdmin => _isInRole(UserRoles.ADMIN.ToString());

        public BaseManager(IHttpContextAccessor httpContextAccessor, IMapper mapper, MRUserManager mrUserManager, IMRUserStore mrUserStore) : base(httpContextAccessor)
        {
            _mapper = mapper;
            _userManager = mrUserManager;
            _userStore = mrUserStore;
        }
    }
}
