using SrcProject.Models.InModels;
using SrcProject.Models.OutModels;
using System.Security;

namespace SrcProject.Services.Contract.Security
{
    public interface IAuthorization_Service
    {
        Task<List<PermissionsByUserByRouteOM>> GetPermissionsByUser(PermissionsIM permissionsIM);
        Task<bool> AddExternalUser(AddExternalUserIM addExternalUserIM);
        Task<List<object>> GetPartners();
        Task<bool> AddPermissionsByUser(PermissionsIM permissionsIM);
        Task<bool> DeletePermissionsByUser(PermissionsIM permissionsIM);
        Task<List<object>> GetRoutes();
        Task<List<object>> GetUser();
    }
}
