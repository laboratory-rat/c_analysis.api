using Infrastructure.Entity.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MRMongoTools.Extensions.Identity.Component;
using MRMongoTools.Extensions.Identity.Interface;
using MRMongoTools.Extensions.Identity.Manager;
using System;
using System.Collections.Generic;
using System.Text;

namespace Manager
{
    public class UserManager : MRUserManager<UserEntity>
    {
        public UserManager(IMRUserStore<UserEntity> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<UserEntity> passwordHasher, IEnumerable<IUserValidator<UserEntity>> userValidators, IEnumerable<IPasswordValidator<UserEntity>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<UserEntity>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }
    }
}
