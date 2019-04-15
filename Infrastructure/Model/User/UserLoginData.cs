using MRMongoTools.Extensions.Identity.Enum;
using System;
using System.Collections.Generic;

namespace Infrastructure.Model.User
{
    public class UserLoginResponse
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public UserSex Sex { get; set; }
        public string ImageUrl { get; set; }
        public List<string> Roles { get; set; }
        public UserLoginResponseToken Token { get; set; }
    }

    public class UserLoginResponseToken
    {
        public string Token { get; set; }
        public DateTime Expire { get; set; }
    }
}
