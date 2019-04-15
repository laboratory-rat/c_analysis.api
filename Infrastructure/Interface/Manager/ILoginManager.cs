using Infrastructure.Model.User;
using MRIdentityClient.Response;
using System.Threading.Tasks;

namespace Infrastructure.Interface.Manager
{
    public interface ILoginManager
    {
        Task<UserLoginResponse> Approve(string token);
        Task<ApiOkResult> Logout();
        ApiOkResult PingLogin();
    }
}
