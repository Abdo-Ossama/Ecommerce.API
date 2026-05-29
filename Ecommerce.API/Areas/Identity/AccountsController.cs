
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Areas.Identity
{
    [Route("[area]/[controller]")]
    [ApiController]
    [Area(SD.IDENTITY)]

    public class AccountsController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IAccountService _accountService;
        private readonly IRepository<ApplicationUserOTP> _applicationUserOTPRepository;
        private readonly IJWTHandler _jwtHandler;

        public AccountsController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager
            , IEmailSender emailSender
            , IAccountService accountService
            , IJWTHandler jwtHandler
            , IRepository<ApplicationUserOTP> applicationUserOTPRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _accountService = accountService;
            _applicationUserOTPRepository = applicationUserOTPRepository;
            _jwtHandler = jwtHandler;
        }





        [HttpPost("Logout")]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();

            //return RedirectToAction("Login", "Account", new { area = "Identity" });
            return Ok(new SuccessResponse<bool>
            {
                Msg = "Logout successful", 
                Data = true 
            });
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {

            // ========Using Mapster ==========..

            var applicationUser = registerRequest.Adapt<ApplicationUser>();

            var result = await _userManager.CreateAsync(applicationUser, registerRequest.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors
                    .Select(e => e.Description)
                    .ToList();

                return BadRequest(new ErrorResponse
                {
                    Message = "Registration Failed",
                    Errors = errors
                });
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
            var ConfirmLink = Url.Action("Confirm", "Accounts", new { area = SD.IDENTITY, token, userId = applicationUser.Id }, Request.Scheme);
            if (ConfirmLink is null)
            {
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Failed to generate confirmation link"
                });
            }

            await _accountService.sendEmailAsync(EmailType.ConfirmEmail, applicationUser, $"Click Here for Confirm Email {ConfirmLink}");
            await _userManager.AddToRoleAsync(applicationUser, SD.CUSTOMER_ROLE);
            return Ok(new SuccessResponse<bool>
            {
                Msg = "Your Register is Completed Successfully ..",
                Data = true
            });
        }

        [HttpGet("Confirm")]
        public async Task<IActionResult> Confirm(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();
            await _userManager.ConfirmEmailAsync(user, token);
            return Ok(new SuccessResponse<bool>
            {
                Msg = "Email Confirmed Successfully .",
                Data = true

            });

        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.EmailOrUserName) ??
                       await _userManager.FindByNameAsync(loginRequest.EmailOrUserName);

            if (user is null)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Invalid username/email or password"
                });
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                loginRequest.Password,
                loginRequest.RememberMe,
                false
            );

            if (result.IsNotAllowed)
            {
                return Unauthorized(new ErrorResponse { Message = "Confirm email first" });
            }

            if (result.IsLockedOut)
            {
                return Unauthorized(new ErrorResponse
                {
                    Message = "Account is locked due to multiple failed attempts"
                });
            }

            if (!result.Succeeded)
            {
                return Unauthorized(new ErrorResponse
                {
                    Message = "Invalid username/email or password"
                });
            }

            var roles = await _userManager.GetRolesAsync(user);

            var token = await _jwtHandler.GenerateTokenAsync(user.Id, user.Email);

            return Ok(new AuthResponse
            {
                Msg = $"Welcome {user.Fname}",
                Token = token,
                UserId = user.Id
            });
        }


        [HttpPost("Resend-Email-Confirmation")]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationRequest resendEmailConfirmationrequest)
        {

            var user = await _userManager.FindByEmailAsync(resendEmailConfirmationrequest.UsernameOrEmail) ??
              await _userManager.FindByNameAsync(resendEmailConfirmationrequest.UsernameOrEmail);
            if (user is not null && !user.EmailConfirmed)
            {
                //لازم يبقى معملش Email Confirmed قبل كدا عشان بقدر يعمل  

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var ConfirmLink = Url.Action(
            "Confirm",
            "Accounts",
            new { area = "Identity", token, userId = user.Id },
            Request.Scheme
        );

                if (ConfirmLink is null)
                {
                    return StatusCode(500, new ErrorResponse
                    {
                        Message = "Failed to generate confirmation link"
                    });
                }
                await _accountService.sendEmailAsync(EmailType.ResendEmailConfirmation, user, $"Click Here to Reconfirm the email {ConfirmLink}");

            }

            return Ok(new SuccessResponse<bool>
            {
                Msg = "Reconfirmed Email Succsesfull",
                Data = true
            });
        }
    


    [HttpPost("Forget-Password")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordRequest forgetPasswordrequest)
        {

            var user = await _userManager.FindByEmailAsync(forgetPasswordrequest.UsernameOrEmail) ??
               await _userManager.FindByNameAsync(forgetPasswordrequest.UsernameOrEmail);

            if (user is null) return NotFound();
            var otp = new Random().Next(1000, 9999).ToString();
            await _accountService.sendEmailAsync(EmailType.ForgetPassword, user!, $"This is the OTP : {otp} Please Don`t Share it ! ");
            var otpsCount = (await _applicationUserOTPRepository
                .GetAsync(e => e.ApplicationUserId == user.Id && e.CreatedAt >= DateTime.Now.AddHours(-24)))
                .Count();
            if (user is not null && otpsCount < 50)
            {
                await _applicationUserOTPRepository.CreateAsync(new ApplicationUserOTP
                {
                    ApplicationUserId = user.Id,
                    OTP = otp
                });
            }
            else if (otpsCount > 50)
            {

                return BadRequest(new ErrorResponse
                {
                    Message = "Many Attemps Today , Please Try Again !"
                });
            }

            await _applicationUserOTPRepository.CommitAsync();
            return Ok(new SuccessResponse<bool>
            {
                Msg = "The OTP Sent to Email Successfully !",
                Data = true
            });
        }


        [HttpPost("Validate-OTP")]
        public async Task<IActionResult> ValidateOTP(ValidateOTPRequest validateOTPRequest )
        {
           
            var user = await _userManager.FindByIdAsync(validateOTPRequest.ApplicationUserId);
            if (user is null) return NotFound();
            // محتاج اجيب الotp اللي ف الداتا بيز 
            var otp = (await _applicationUserOTPRepository.GetAsync()).Where(e => e.IsValid && e.ApplicationUser.Id == user.Id)
                .OrderBy(e => e.Id).LastOrDefault();
            if (otp is null || otp.OTP != validateOTPRequest.OTP)
            {
                return BadRequest(new ErrorResponse
               {
                   Message = "Invalid OTP , Please Try Again !"
               });
            }
            else
            {
                otp.IsUsed = true;
                _applicationUserOTPRepository.Update(otp);
                await _applicationUserOTPRepository.CommitAsync();
                return Ok(new SuccessResponse<bool>
                {
                    Msg = "OTP verified successfully",
                    Data = true
                });
            }

        }

        [HttpPost("Reset-Password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest resetPasswordRequest )
        {
           
            var user = await _userManager.FindByIdAsync(resetPasswordRequest.ApplicationUserId);
            if (user is null) return NotFound();

            var userToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, userToken, resetPasswordRequest.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(new ErrorResponse()
                {
                    Errors = errors
                });
            }

            return Ok(new SuccessResponse<bool>
            {
                Msg = "Change Password Successfully",
                Data = true
            });

        }
    }




};