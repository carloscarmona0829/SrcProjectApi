using SrcProject.Models.InModels.Security;
using SrcProject.Models.OutModels.Security;
using System.Security;

namespace SrcProject.Services.Contract.Security
{
    public interface IAuthorization_Service
    {
        Task<List<PermissionsByUserByRouteOM>> GetPermissionsByUser(PermissionsIM permissionsIM);
        Task<bool> AddExternalUser(AddExternalUserIM addExternalUserIM);
        Task<bool> AddPermissionsByUser(PermissionsIM permissionsIM);
        Task<bool> DeletePermissionsByUser(PermissionsIM permissionsIM);
        Task<List<object>> GetRoutes();
        Task<List<object>> GetUser();
    }
}
