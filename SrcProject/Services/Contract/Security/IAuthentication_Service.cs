using SrcProject.Models.InModels.Security;
using SrcProject.Models.OutModels.Security;
using SrcProject.Utilities;

namespace SrcProject.Services.Contract.Security
{
    public interface IAuthentication_Service
    {
        Task<ResponseManager> Register(RegisterModelIM registerModelIM);
        Task<ResponseManager> Login(LoginIM loginIM);
        Task<ResponseManager> GetPermissionsByUser(LoginIM loginIM);
        Task<ResponseManager> ConfirmEmail(string userId, string token);
        Task<ResponseManager> ForgetPassword(string email);
        Task<ResponseManager> ResetPassword(ResetPasswordIM resetPasswordIM);
    }
}
