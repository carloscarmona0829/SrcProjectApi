using SrcProject.Models.InModels.Security;
using SrcProject.Models.OutModels.Security;
using SrcProject.Utilities;
using System.Security;

namespace SrcProject.Services.Contract.Security
{
    public interface IAuthorization_Service
    {
        Task<List<PermissionsByUserByRouteOM>> GetPermissionsByUserByRoute(PermissionsIM permissionsIM);
        Task<ResponseManager> AddPermissionsByUser(PermissionsIM permissionsIM);
        Task<ResponseManager> DeletePermissionsByUser(PermissionsIM permissionsIM);
        Task<List<object>> GetRoutes();
        Task<List<object>> GetUser();
    }
}
