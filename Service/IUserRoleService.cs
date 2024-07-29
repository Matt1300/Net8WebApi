using LearnAPI.Helper;
using LearnAPI.Modal;
using LearnAPI.Repos.Models;

namespace LearnAPI.Service
{
    public interface IUserRoleService
    {
        Task<APIResponse> AssignRolPermission(List<TblRolepermission> _data);
        Task<List<TblRole>> GetAllRoles();
        Task<List<TblMenu>> GetAllMenus();
        Task<List<Appmenu>> GetAllMenusByRol(string userRol);
        Task<MenuPermission> GetMenuPermissionByRol(string userRol, string menucode);
    }
}
