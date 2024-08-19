using LearnAPI.Helper;
using LearnAPI.Modal;

namespace LearnAPI.Service
{
    public interface IUserService
    {
        Task<APIResponse> UserRegistration(UserRegister userRegister);
        Task<APIResponse> ConfirmRegistration(RegisterConfirm registerConfirm);
        Task<APIResponse> ResetPassword(ResetPassword resetPassword);
        Task<APIResponse> ForgetPassword(string username);
        Task<APIResponse> UpdatePassword(UpdatePassword updatePassword);
        Task<APIResponse> UpdateStatus(string username, bool userStatus);
        Task<APIResponse> UpdateRol(string username, string userRol);

    }
}
