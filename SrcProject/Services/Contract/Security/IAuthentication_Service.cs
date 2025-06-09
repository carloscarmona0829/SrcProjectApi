using SrcProject.Models.InModels.Security;
using SrcProject.Models.OutModels;
using SrcProject.Models.OutModels.Security;

namespace SrcProject.Services.Contract.Security
{
    public interface IAuthentication_Service
    {
        Task<ResponseManagerOM> Register(RegisterModelIM registerModelIM);
        Task<ResponseManagerOM> Login(LoginIM loginIM);
        Task<ResponseManagerOM> GetPermissionsByUser(LoginIM loginIM);
        Task<ResponseManagerOM> ConfirmEmail(string userId, string token);
        Task<ResponseManagerOM> ForgetPassword(string email);
        Task<ResponseManagerOM> ResetPassword(ResetPasswordIM resetPasswordIM);
    }
}
