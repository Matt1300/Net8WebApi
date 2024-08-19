using LearnAPI.Helper;
using LearnAPI.Modal;
using LearnAPI.Repos;
using LearnAPI.Repos.Models;
using LearnAPI.Service;
using Microsoft.EntityFrameworkCore;

namespace LearnAPI.Container
{
    public class UserService : IUserService
    {
        private readonly LearndataContext _context;
        public UserService(LearndataContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> ConfirmRegistration(RegisterConfirm registerConfirm)
        {
            APIResponse response = new APIResponse();
            bool otpResponse = await ValidateOTP(registerConfirm.Username, registerConfirm.OtpText);
            if (!otpResponse)
            {
                response.ResponseCode = 400;
                response.ErrorMessage = "Invalid OTP or Expired";
            }
            else
            {
                var _tempdata = await _context.TblTempusers.FindAsync(registerConfirm.UserId);
                var _user = new TblUser()
                {
                    Username = registerConfirm.Username,
                    Name = _tempdata!.Name,
                    Email = _tempdata.Email,
                    Password = _tempdata.Password,
                    Phone = _tempdata.Phone,
                    Failattempt = 0,
                    Isactive = true,
                    Islocked = false,
                    Role = "user"
                };
                _context.TblUsers.Add(_user);
                await _context.SaveChangesAsync();
                await UpdatePWDManager(registerConfirm.Username, _tempdata.Password);

                response.ResponseCode = 200;
                response.Result = "Registered successfully.";
            }

            return response;
        }

        public async Task<APIResponse> UserRegistration(UserRegister userRegister)
        {
            APIResponse response = new APIResponse();
            int userId = 0;
            string OTPText = string.Empty;
            bool isValid = true;

            try
            {
                //Duplicate user
                var _user = await _context.TblUsers.Where(item => item.Username == userRegister.UserName).ToListAsync();
                if (_user.Count > 0)
                {
                    isValid = false;
                    response.ResponseCode = 400;
                    response.ErrorMessage = "Duplicate username";
                }

                //Duplicate email
                var _userEmail = await _context.TblUsers.Where(item => item.Email == userRegister.Email).ToListAsync();
                if (_userEmail.Count > 0)
                {
                    isValid = false;
                    response.ResponseCode = 400;
                    response.ErrorMessage = "Duplicate email";
                }

                if (userRegister != null && isValid)
                {
                    var _tempUser = new TblTempuser()
                    {
                        Code = userRegister.UserName,
                        Name = userRegister.Name,
                        Email = userRegister.Email,
                        Password = userRegister.Password,
                        Phone = userRegister.Phone,
                    };
                    await _context.TblTempusers.AddAsync(_tempUser);
                    await _context.SaveChangesAsync();

                    userId = _tempUser.Id;
                    OTPText = GenerateRandomNumber();
                    await UpdateOtp(userRegister.UserName, OTPText, "register");
                    SendOtpMail(userRegister.Email, OTPText, userRegister.Name);

                    response.ResponseCode = 200;
                    response.Result = userId.ToString();
                }
            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.ErrorMessage = "Fail: " + ex;
            }

            return response;
        }

        public async Task<APIResponse> ResetPassword(ResetPassword resetPassword)
        {
            APIResponse response = new APIResponse();

            var _user = await _context.TblUsers.FirstOrDefaultAsync(item => item.Username == resetPassword.Username && item.Password == resetPassword.OldPassword && item.Isactive == true);
            if (_user != null)
            {
                var _pwdHistory = await ValidatePWDHistory(resetPassword.Username, resetPassword.NewPassword);
                if (_pwdHistory)
                {
                    response.ResponseCode = 400;
                    response.ErrorMessage = "Don't use the same password that used in last 3 transaction";
                }
                else
                {
                    _user.Password = resetPassword.NewPassword;
                    await _context.SaveChangesAsync();
                    await UpdatePWDManager(resetPassword.Username, resetPassword.NewPassword);

                    response.ResponseCode = 200;
                    response.Result = "Password updated";
                }


            }
            else
            {
                response.ResponseCode = 400;
                response.ErrorMessage = "Failed to validate old password";
            }

            return response;
        }

        public async Task<APIResponse> ForgetPassword(string username)
        {
            APIResponse response = new APIResponse();

            var _user = await _context.TblUsers.FirstOrDefaultAsync(item => item.Username == username && item.Isactive == true);
            if (_user != null)
            {
                string otptext = GenerateRandomNumber();
                await UpdateOtp(username, otptext, "forgetpassword");
                if (_user.Email != null)
                    await SendOtpMail(_user.Email, otptext, _user.Username);

                response.ResponseCode = 200;
                response.Result = "OTP Sent";
            }
            else
            {
                response.ResponseCode = 400;
                response.ErrorMessage = "Invalid User";
            }

            return response;
        }

        public async Task<APIResponse> UpdatePassword(UpdatePassword updatePassword)
        {
            APIResponse response = new APIResponse();

            bool otpValidation = await ValidateOTP(updatePassword.Username, updatePassword.OtpText);
            if (otpValidation)
            {
                bool pwdHistory = await ValidatePWDHistory(updatePassword.Username, updatePassword.Password);
                if (pwdHistory)
                {
                    response.ResponseCode = 400;
                    response.ErrorMessage = "Don't use the same password that used in last 3 transaction";
                }
                else
                {
                    var _user = await _context.TblUsers.FirstOrDefaultAsync(item => item.Username == updatePassword.Username && item.Isactive == true);
                    if (_user != null)
                    {
                        _user.Password = updatePassword.Password;
                        await _context.SaveChangesAsync();
                        await UpdatePWDManager(updatePassword.Username, updatePassword.Password);

                        response.ResponseCode = 200;
                        response.Result = "Password Updated";
                    }
                }
            }
            else
            {
                response.ResponseCode = 400;
                response.ErrorMessage = "Invalid OTP";
            }

            return response;
        }
        public async Task<APIResponse> UpdateStatus(string username, bool userStatus)
        {
            APIResponse response = new APIResponse();
            var _user = await _context.TblUsers.FirstOrDefaultAsync(item => item.Username == username);
            if (_user != null)
            {
                _user.Isactive = userStatus;
                await _context.SaveChangesAsync();

                response.ResponseCode = 200;
                response.Result = "Status Updated";
            }
            else
            {
                response.ResponseCode = 400;
                response.ErrorMessage = "Invalid user";
            }
            return response;
        }

        public async Task<APIResponse> UpdateRol(string username, string userRol)
        {
            APIResponse response = new APIResponse();
            var _user = await _context.TblUsers.FirstOrDefaultAsync(item => item.Username == username && item.Isactive == true);
            if (_user != null)
            {
                _user.Role = userRol;
                await _context.SaveChangesAsync();

                response.ResponseCode = 200;
                response.Result = "Rol Updated";
            }
            else
            {
                response.ResponseCode = 400;
                response.ErrorMessage = "Invalid user";
            }


            return response;
        }

        private async Task UpdateOtp(string username, string otpText, string otpType)
        {
            var _otp = new TblOtpManager()
            {
                Username = username,
                Otptext = otpText,
                Expiration = DateTime.Now.AddMinutes(30),
                Createddate = DateTime.Now,
                Otptype = otpType
            };

            await _context.TblOtpManagers.AddAsync(_otp);
            await _context.SaveChangesAsync();
        }

        private async Task<bool> ValidateOTP(string username, string OTPText)
        {
            bool response = false;
            var _data = await _context.TblOtpManagers.FirstOrDefaultAsync(item => item.Username == username && item.Otptext == OTPText && item.Expiration > DateTime.Now);
            if (_data != null)
            {
                response = true;
            }
            return response;
        }

        private async Task UpdatePWDManager(string username, string password)
        {
            var _otp = new TblPwdManger()
            {
                Username = username,
                Password = password,
                ModifyDate = DateTime.Now
            };

            await _context.TblPwdMangers.AddAsync(_otp);
            await _context.SaveChangesAsync();
        }

        private string GenerateRandomNumber()
        {
            Random random = new Random();
            string randomno = random.Next(0, 100000).ToString("D6");
            return randomno;
        }

        private async Task SendOtpMail(string userEmail, string otpText, string name)
        {

        }

        private async Task<bool> ValidatePWDHistory(string username, string password)
        {
            bool response = false;
            var _pwd = await _context.TblPwdMangers.Where(item => item.Username == username).OrderByDescending(p => p.ModifyDate).Take(3).ToListAsync();
            if (_pwd.Count > 0)
            {
                var validate = _pwd.Where(o => o.Password == password);
                if (validate.Any())
                {
                    response = true;
                }
            }
            return response;
        }


    }
}
