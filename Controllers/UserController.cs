using LearnAPI.Modal;
using LearnAPI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearnAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("UserRegistration")]
        public async Task<IActionResult> UserRegistration(UserRegister userRegister)
        {
            var data = await _userService.UserRegistration(userRegister);
            return Ok(data);
        }

        [HttpPost("ConfirmRegistration")]
        public async Task<IActionResult> ConfirmRegistration(RegisterConfirm registerConfirm)
        {
            var data = await _userService.ConfirmRegistration(registerConfirm);
            return Ok(data);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            var data = await _userService.ResetPassword(resetPassword);
            return Ok(data);
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string username)
        {
            var data = await _userService.ForgetPassword(username);
            return Ok(data);
        }

        [HttpPost("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword(UpdatePassword updatePassword)
        {
            var data = await _userService.UpdatePassword(updatePassword);
            return Ok(data);
        }

        [HttpPost("UpdateRol")]
        public async Task<IActionResult> UpdateRol(string username, string userRol)
        {
            var data = await _userService.UpdateRol(username, userRol);
            return Ok(data);
        }

        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(string username, bool userStatus)
        {
            var data = await _userService.UpdateStatus(username, userStatus);
            return Ok(data);
        }

    }
}
