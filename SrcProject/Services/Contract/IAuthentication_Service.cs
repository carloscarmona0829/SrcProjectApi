using SrcProject.Models.InModels;
using SrcProject.Models.InModels.Security;
using SrcProject.Models.OutModels;
using SrcProject.Utilities;

namespace SrcProject.Services.Contract
{
    public interface IAuthentication_Service
    {
        Task<ResponseManager> RegisterAsync(RegisterModelIM registerModelIM);
        Task<ResponseManager> Login(LoginIM loginIM);
        Task<List<PermissionsOM>> GetPermissionsByUser(string strUserName);

    }
}
