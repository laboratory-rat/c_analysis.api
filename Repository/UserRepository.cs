using Infrastructure.Entity.User;
using Infrastructure.Interface.Repository;
using MRMongoTools.Extensions.Identity.Store;
using MRMongoTools.Infrastructure.Settings;

namespace Repository
{
    public class UserRepository : MRUserStore<UserEntity>, IUserRepository
    {
        public UserRepository(MRDatabaseConnectionSettings settings) : base(settings) { }
    }
}
