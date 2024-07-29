using LearnAPI.Modal;
using LearnAPI.Repos.Models;
using LearnAPI.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearnAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRolController : ControllerBase
    {
        private readonly IUserRoleService _roleService;
        public UserRolController(IUserRoleService roleService   ) 
        { 
            _roleService = roleService;
        }

        [HttpPost("AssignRolPermission")]
        public async Task<IActionResult> AssignRolPermission(List<TblRolepermission> _data)
        {
            var data = await _roleService.AssignRolPermission(_data);
            return Ok(data);
        }

        [HttpGet("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var data = await _roleService.GetAllRoles();
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }
        
        [HttpGet("GetAllMenus")]
        public async Task<IActionResult> GetAllMenus()
        {
            var data = await _roleService.GetAllMenus();
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet("GetAllMenusByRol")]
        public async Task<IActionResult> GetAllMenusByRol(string userRol)
        {
            var data = await _roleService.GetAllMenusByRol(userRol);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet("GetMenuPermissionByRol")]
        public async Task<IActionResult> GetMenuPermissionByRol(string userRol, string menucode)
        {
            var data = await _roleService.GetMenuPermissionByRol(userRol, menucode);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }
    }
}
