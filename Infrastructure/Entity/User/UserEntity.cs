using MRMongoTools.Extensions.Identity.Component;
using MRMongoTools.Infrastructure.Attr;

namespace Infrastructure.Entity.User
{
    [CollectionAttr("User")]
    public class UserEntity : MRUser
    {
        public string ServiceId { get; set; }
    }
}
