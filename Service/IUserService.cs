using LearnAPI.Helper;
using LearnAPI.Modal;

namespace LearnAPI.Service
{
    public interface IUserService
    {
        Task<APIResponse> UserRegistration(UserRegister userRegister);
        Task<APIResponse> ConfirmRegistration(int userId, string username, string otpText);
        Task<APIResponse> ResetPassword(string username, string oldPassword, string newPassword);
        Task<APIResponse> ForgetPassword(string username);
        Task<APIResponse> UpdatePassword(string username, string password, string otpText);
        Task<APIResponse> UpdateStatus(string username, bool userStatus);
        Task<APIResponse> UpdateRol(string username, string userRol);

    }
}
