using LearnAPI.Helper;
using LearnAPI.Modal;
using LearnAPI.Repos;
using LearnAPI.Repos.Models;
using LearnAPI.Service;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace LearnAPI.Container
{
    public class UserRoleService : IUserRoleService
    {
        private readonly LearndataContext _context = new LearndataContext();

        public UserRoleService(LearndataContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> AssignRolPermission(List<TblRolepermission> _data)
        {
            APIResponse response = new APIResponse();
            int processcount = 0;
            try
            {
                using (var dbtransaction = await _context.Database.BeginTransactionAsync())
                {
                    if (_data.Count > 0)
                    {
                        foreach (var item in _data)
                        {
                            var userdata = await _context.TblRolepermissions.FirstOrDefaultAsync(x => x.Userrole == item.Userrole && x.Menucode == item.Menucode);
                            if (userdata != null)
                            {
                                userdata.Haveview = item.Haveview;
                                userdata.Haveadd = item.Haveadd;
                                userdata.Haveedit = item.Haveedit;
                                userdata.Havedelete = item.Havedelete;
                                processcount++;
                            }
                            else
                            {
                                _context.TblRolepermissions.Add(item);
                                processcount++;
                            }
                        }

                        if (_data.Count == processcount)
                        {
                            await _context.SaveChangesAsync();
                            await dbtransaction.CommitAsync();
                            response.ResponseCode = 200;
                            response.Result = "Saved successfully";
                        }
                        else
                        {
                            await dbtransaction.RollbackAsync();
                        }
                    }
                    else
                    {
                        response.ResponseCode = 400;
                        response.ErrorMessage = "Failed";
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return response;
        }

        public async Task<List<TblMenu>> GetAllMenus()
        {
            return await _context.TblMenus.ToListAsync();
        }

        public async Task<List<Appmenu>> GetAllMenusByRol(string userRol)
        {
            List<Appmenu> appmenus = new List<Appmenu>();

            var accessData = await (from menu in _context.TblRolepermissions.Where(o => o.Userrole == userRol && o.Haveview)
                              join m in _context.TblMenus on menu.Menucode equals m.Code into _jointable
                              from p in _jointable.DefaultIfEmpty() 
                              select new {code = menu.Menucode, name = p.Name}).ToListAsync();

            if(accessData.Any())
            {
                accessData.ForEach(item =>
                {
                    appmenus.Add(new Appmenu()
                    {
                        Code = item.code,
                        Name = item.name
                    });
                });
            }

            return appmenus;
        }

        public async Task<List<TblRole>> GetAllRoles()
        {
            return await _context.TblRoles.ToListAsync();
        }

        public async Task<MenuPermission> GetMenuPermissionByRol(string userRol, string menucode)
        {
            MenuPermission menuPermission = new MenuPermission();
            var _data = await _context.TblRolepermissions.FirstOrDefaultAsync(o => o.Userrole == userRol && o.Menucode == menucode);
            if (_data != null)
            {
                menuPermission.Code = _data.Menucode;
                menuPermission.Haveadd = _data.Haveadd;
                menuPermission.Haveview = _data.Haveview;
                menuPermission.Haveedit = _data.Haveedit;
                menuPermission.Havedelete = _data.Havedelete;
            }

            return menuPermission;
        }
    }
}
