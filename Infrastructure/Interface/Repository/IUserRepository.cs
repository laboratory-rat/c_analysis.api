using Infrastructure.Entity.User;
using MRMongoTools.Extensions.Identity.Interface;

namespace Infrastructure.Interface.Repository
{
    public interface IUserRepository : IMRUserStore<UserEntity>
    {
    }
}
