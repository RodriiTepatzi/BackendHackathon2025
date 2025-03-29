using BackendHackathon2025.Data.Base.Models;
using BackendHackathon2025.Data.Const;
using BackendHackathon2025.Data.DTOs;
using BackendHackathon2025.Data.Interfaces;
using BackendHackathon2025.Data.Models;
using BackendHackathon2025.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BackendHackathon2025.Controllers
{
	[Route("api/v1/auth")]
	[ApiController]
	public class AuthController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		//private readonly VonageClient _vonageClient;
		private readonly TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
		private readonly IVerificationCodeService _verificationCodeService;

		public AuthController(UserManager<ApplicationUser> userManager, IVerificationCodeService verificationCodeService)
		{
			_userManager = userManager;
			_verificationCodeService = verificationCodeService;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterModel model)
		{
			if (model == null || !ModelState.IsValid)
				return BadRequest(new BaseResponse<RegisterModel> { Data = null, Error = ResponseErrors.AuthInvalidData });

			if (await _userManager.FindByEmailAsync(model.Email!) != null)
				return BadRequest(new BaseResponse<RegisterModel> { Data = null, Error = ResponseErrors.AuthUserEmailAlreadyExists });

			var existingUserWithPhone = await _userManager.Users.AnyAsync(u => u.PhoneNumber == model.PhoneNumber);


			if (existingUserWithPhone)
			{

				var user = await _userManager.Users.FirstOrDefaultAsync(u =>
					u.PhoneNumber == model.PhoneNumber
				);


				if (user is null)
				{
					return NotFound(new BaseResponse<Object> { Error = ResponseErrors.AuthUserNotFound });
				}

				var expirationDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

				var verificationCode = await _verificationCodeService.GetAllAsync(vc =>
					vc.Code == model.VerificationCode &&
					vc.UserId == user.Id &&
					vc.ExpirationDate >= expirationDate
				);

				if (!verificationCode.Any())
				{
					return BadRequest(new BaseResponse<Object> { Error = ResponseErrors.AuthInvalidVerificationCode });
				}

				user.Name = model.Name;
				user.LastName = model.LastName;
				user.Email = model.Email;
				user.Birthday = model.Birthday;


				var result = await _userManager.UpdateAsync(user);
				var resultPassword = await _userManager.AddPasswordAsync(user, model.Password!);

				if (!result.Succeeded && !resultPassword.Succeeded)
					return BadRequest(new BaseResponse<RegisterModel> { Data = null, Error = ResponseErrors.AuthErrorUpdatingUser });

				var accessToken = GenerateJwtToken(user);
				var rememberMe = true;
				var refreshToken = GenerateRefreshToken();


				if (rememberMe == true) user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(180);
				if (rememberMe == false) user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(120);

				user.PhoneNumberConfirmed = true;
				user.IsActive = true;
				user.RefreshToken = refreshToken;

				await _userManager.UpdateAsync(user); 

				verificationCode.First().IsUsed = true;
				await _verificationCodeService.UpdateAsync(verificationCode.First());


				return Ok(new BaseResponse<TokenRefreshModel>
                {
                    Data = new TokenRefreshModel
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        ExpiryDate = user.RefreshTokenExpiryTime,
                    }
                });
            
			}else
			{
                return NotFound(new BaseResponse<Object> { Error = ResponseErrors.AuthUserNotFound });
            }
		}


		[HttpPost("check-phone")]
		public async Task<IActionResult> CheckPhone([FromBody] CheckPhoneModel model)
		{
			if (model == null || !ModelState.IsValid)
				return BadRequest(new BaseResponse<string> { Error = ResponseErrors.AuthInvalidData });

			var userExists = await _userManager.Users.AnyAsync(u => 
				u.PhoneNumber == model.PhoneNumber && 
				u.PhoneCode == model.PhoneCode
			);

			if (userExists)
			{
				var verificationCode = GenerateVerificationCode();

				var user = await _userManager.Users.FirstOrDefaultAsync(u => 
					u.PhoneNumber == model.PhoneNumber && 
					u.PhoneCode == model.PhoneCode && 
					u.UserName == $"{model.PhoneCode}{model.PhoneNumber}"
				);

				if (user is null) return Ok(new BaseResponse<bool> { Data = false });

				var expirationDateFilter = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
				var creationDateFilter = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddMinutes(-10), timeZone);

				var verificationCodeResent = await _verificationCodeService.GetAllAsync(vc =>
					vc.UserId == user.Id &&
					vc.CreationDate >= creationDateFilter
				);

				if (verificationCodeResent.Count() > 4)
				{
					return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AuthTooManyTries });
				}


				var result = await _userManager.UpdateAsync(user);

				if (!result.Succeeded) return BadRequest(new BaseResponse<RegisterModel> { Data = null, Error = ResponseErrors.AuthErrorUpdatingUser });

				var creationDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
				var expirationDate = creationDate.AddMinutes(2);

				var verificationCodeModel = new VerificationCode
				{
					Code = verificationCode,
					UserId = user.Id,
					CreationDate = creationDate,
					ExpirationDate = expirationDate,
					IsUsed = false,
				};

				await _verificationCodeService.AddAsync(verificationCodeModel);

				// var response = await _vonageClient.SmsClient.SendAnSmsAsync(new Vonage.Messaging.SendSmsRequest()
				// {
				// 	To = $"{model.PhoneCode}{model.PhoneNumber}",
				// 	From = "Mandaditos",
				// 	Text = $"Tú codigo de verificación para Mandaditos {verificationCode}"
				// });

				return Ok(new BaseResponse<bool> { Data = true });
			}
			else
			{
				var verificationCode = GenerateVerificationCode();

				var user = new ApplicationUser
				{
					PhoneNumber = model.PhoneNumber,
					UserName = $"{model.PhoneCode}{model.PhoneNumber}",
					PhoneCode = model.PhoneCode,
				};

				var result = await _userManager.CreateAsync(user);

				if (!result.Succeeded) return BadRequest(new BaseResponse<RegisterModel> { Data = null, Error = ResponseErrors.AuthErrorCreatingUser });


				var creationDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
				var expirationDate = creationDate.AddMinutes(2);

				var verificationCodeModel = new VerificationCode
				{
					Code = verificationCode,
					UserId = user.Id,
					CreationDate = creationDate,
					ExpirationDate = expirationDate,
					IsUsed = false,
				};

				await _verificationCodeService.AddAsync(verificationCodeModel);

				// var response = await _vonageClient.SmsClient.SendAnSmsAsync(new Vonage.Messaging.SendSmsRequest()
				// {
				// 	To = $"{model.PhoneCode}{model.PhoneNumber}",
				// 	From = "Mandaditos",
				// 	Text = $"Este es tu codigo de verificacion para mandaditos {verificationCode}"
				// });

				return Ok(new BaseResponse<bool> { Data = true });
			}
		}

		[HttpPost("verify-phone")]
		public async Task<IActionResult> VerifyPhone([FromBody] VerifyPhoneModel model)
		{
			if (model == null)
				return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AuthInvalidData });

			var user = await _userManager.Users.FirstOrDefaultAsync(u => 
				u.PhoneNumber == model.PhoneNumber && 
				u.PhoneCode == model.PhoneCode
			);

			if (user == null)
				return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AuthUserNotFound });

			var verificationCode = await _verificationCodeService.GetAllAsync(vc =>
				vc.Code == model.VerificationCode &&
				vc.UserId == user.Id &&
				vc.IsUsed == false
			);

			if (!verificationCode.Any())
			{
				return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AuthInvalidVerificationCode });
			}



			if (verificationCode.First().Code != model.VerificationCode)
				return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AuthInvalidVerificationCode });


			if (TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone) > verificationCode.First().ExpirationDate)
			{
				return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AuthVerificationCodeExpired });
			}


			if (user.PhoneNumberConfirmed)
			{
				var accessToken = GenerateJwtToken(user);
				var refreshToken = GenerateRefreshToken();

				var rememberMe = true;


				if (rememberMe == true) user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(180);
				if (rememberMe == false) user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(120);

				user.RefreshToken = refreshToken;

				await _userManager.UpdateAsync(user);

				verificationCode.First().IsUsed = true;

				await _verificationCodeService.UpdateAsync(verificationCode.First());

				return Ok(new BaseResponse<TokenRefreshModel>
				{

						Data = new TokenRefreshModel
						{
							AccessToken = accessToken,
							RefreshToken = refreshToken,
							ExpiryDate = user.RefreshTokenExpiryTime,
						}
					
				});
			}
			else {
				user.PhoneNumberConfirmed = false;
				await _userManager.UpdateAsync(user);

				verificationCode.First().ExpirationDate = DateTime.UtcNow.AddMinutes(15);

				await _verificationCodeService.UpdateAsync(verificationCode.First());

				return Ok(new BaseResponse<TokenRefreshModel> {
						Data = null
					} 
				);
			}
		}

		[HttpPost("resend-verification-code")]
		public async Task<IActionResult> ResendVerificationCode([FromBody] ResendVerificationModel model)
		{
			if (model == null || string.IsNullOrEmpty(model.PhoneNumber) || string.IsNullOrEmpty(model.PhoneCode))
				return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AuthInvalidData });

			var user = await _userManager.Users.FirstOrDefaultAsync(u => 
				u.PhoneNumber == model.PhoneNumber && 
				u.PhoneCode == model.PhoneCode
			);

			if (user == null)
				return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AuthUserNotFound });

			var creationDateFilter = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddMinutes(-10), timeZone);
			var expirationDateFilter = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

			var verificationCodeResent = await _verificationCodeService.GetAllAsync(vc =>
					vc.UserId == user.Id &&
					vc.CreationDate >= creationDateFilter
				);

			if (verificationCodeResent.Count() > 4)
			{
				return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AuthTooManyTries });
			}

			var newVerificationCode = GenerateVerificationCode();

			var creationDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
			var expirationDate = creationDate.AddMinutes(2);

			var verificationCodeModel = new VerificationCode
			{
				Code = newVerificationCode,
				UserId = user.Id,
				CreationDate = creationDate,
				ExpirationDate = expirationDate,
				IsUsed = false,
			};

			var updateResult = await _userManager.UpdateAsync(user);
			if (!updateResult.Succeeded)
				return BadRequest(new BaseResponse<bool> {Error = ResponseErrors.AuthInvalidData });

			await _verificationCodeService.AddAsync(verificationCodeModel);

			// var response = await _vonageClient.SmsClient.SendAnSmsAsync(new Vonage.Messaging.SendSmsRequest()
			// {
			// 	To = $"{user.PhoneCode}{user.PhoneNumber}",
			// 	From = "Mandaditos",
			// 	Text = $"Este es tu nuevo codigo de verificacion para mandaditos: {newVerificationCode}"
			// });

			return Ok(new BaseResponse<bool> { Data = true });
		}


		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginModel model)
		{
			if (model == null || !ModelState.IsValid)
			{
				return BadRequest(new BaseResponse<TokenRefreshModel>
				{
					Error = ResponseErrors.AuthInvalidData
				});
			}

			var user = await _userManager.FindByNameAsync($"{model.PhoneCode}{model.PhoneNumber}");

			if (user != null && await _userManager.CheckPasswordAsync(user, model.Password!))
			{
				var accessToken = GenerateJwtToken(user);
				var refreshToken = GenerateRefreshToken();


				if (model.RememberMe == true) user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(180);
				if (model.RememberMe == false) user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(120);


				user.RefreshToken = refreshToken;

				await _userManager.UpdateAsync(user);

				return Ok(new BaseResponse<TokenRefreshModel>
				{
					Data = new TokenRefreshModel
					{
						AccessToken = accessToken,
						RefreshToken = refreshToken,
						ExpiryDate = user.RefreshTokenExpiryTime,
					}
				});
			}

			return Unauthorized(new BaseResponse<TokenRefreshModel>
			{
				Data = null,
				Error = ResponseErrors.AuthInvalidCredentials
			});
		}

		[HttpPost("refresh")]
		public async Task<IActionResult> Refresh([FromBody] TokenRefreshModel model)
		{
			if (model == null || !ModelState.IsValid)
			{
				return BadRequest(new BaseResponse<TokenRefreshModel>
				{
					Error = ResponseErrors.AuthInvalidData
				});
			}

			var principal = GetPrincipalFromExpiredToken(model.AccessToken!);
			var userId = principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

			if (userId == null)
			{
				return BadRequest(new BaseResponse<TokenRefreshModel>
				{
					Error = ResponseErrors.AuthInvalidToken
				});
			}

			var user = await _userManager.FindByEmailAsync(userId);
			if (user == null || user.RefreshToken != model.RefreshToken)
			{
				return BadRequest(new BaseResponse<TokenRefreshModel>
				{
					Error = ResponseErrors.AuthInvalidRefreshToken
				});
			}

			if (user.RefreshTokenExpiryTime <= DateTime.Now)
			{
				return Unauthorized(new BaseResponse<TokenRefreshModel>
				{
					Error = ResponseErrors.AuthRefreshTokenExpired
				});
			}

			var newAccessToken = GenerateJwtToken(user);

			await _userManager.UpdateAsync(user);

			return Ok(new BaseResponse<TokenRefreshModel>
			{
				Data = new TokenRefreshModel
				{
					AccessToken = newAccessToken,
					RefreshToken = user.RefreshToken,
					ExpiryDate = user.RefreshTokenExpiryTime
				}
			});
		}


		//[HttpPut("update")]
		//[Authorize(Roles = "Admin")]
		//public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateUserSchema model)
		//{
		//	var userEmail = User.FindFirstValue(ClaimTypes.NameIdentifier);
		//	if (userEmail == null)
		//	{
		//		return Unauthorized(new BaseResponse<string>
		//		{
		//			Error = ResponseErrors.AuthInvalidToken
		//		});
		//	}

		//	var user = await _userManager.Users.OfType<Diner>().FirstOrDefaultAsync(u => u.Email == userEmail);
		//	if (user == null)
		//	{
		//		return NotFound(new BaseResponse<string>
		//		{
		//			Error = ResponseErrors.AuthUserNotFound
		//		});
		//	}

		//	user.Name = model.Name;
		//	user.LastName = model.LastName;
		//	user.PhoneNumber = model.PhoneNumber;
		//	user.Email = model.Email;

		//	var result = await _userManager.UpdateAsync(user);
		//	if (!result.Succeeded)
		//	{
		//		return BadRequest(new BaseResponse<string>
		//		{
		//			Error = ResponseErrors.AuthErrorUpdatingUser
		//		});
		//	}

		//	return Ok(new BaseResponse<bool>
		//	{
		//		Data = true
		//	});
		//}

		[HttpGet("me")]
		[Authorize]
		public async Task<IActionResult> GetCurrentUser()
		{
			var userEmail = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userEmail == null)
			{
				return Unauthorized(new BaseResponse<ApplicationUser>
				{
					Error = ResponseErrors.AuthInvalidToken
				});
			}

			var user = await _userManager.Users
				.Where(u => u.Email == userEmail)
				.Select(u => new DinerMeDto
				{
					Id = u.Id,
					Email = u.Email,
					UserName = u.UserName,
					FirstName = u.Name,
					LastName = u.LastName,
					PhoneNumber = u.PhoneNumber,
					IsActive = u.IsActive,
					IsVerified = u.IsVerified,
					PhoneNumberConfirmed = u.PhoneNumberConfirmed,
					Birthday = u.Birthday,
				})
				.FirstOrDefaultAsync();

			if (user == null)
			{
				return NotFound(new BaseResponse<ApplicationUser>
				{
					Error = ResponseErrors.AuthUserNotFound
				});
			}


			return Ok(new BaseResponse<DinerMeDto>
			{
				Data = user,
			});
		}

		[HttpPut("change-password")]
		[Authorize]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
		{
			var userEmail = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userEmail == null)
			{
				return Unauthorized(new BaseResponse<string>
				{
					Error = ResponseErrors.AuthInvalidToken
				});
			}

			var user = await _userManager.FindByEmailAsync(userEmail);
			if (user == null)
			{
				return NotFound(new BaseResponse<string>
				{
					Error = ResponseErrors.AuthUserNotFound
				});
			}

			var passwordCheck = await _userManager.CheckPasswordAsync(user, model.OldPassword!);

			if (!passwordCheck)
			{
				return BadRequest(new BaseResponse<string>
				{
					Error = ResponseErrors.AuthInvalidCurrentPassword
				});
			}

			var result = await _userManager.ChangePasswordAsync(user, model.OldPassword!, model.NewPassword!);
			if (!result.Succeeded)
			{
				return BadRequest(new BaseResponse<string>
				{
					Error = ResponseErrors.AuthErrorChangingPassword
				});
			}

			return Ok(new BaseResponse<string>
			{
				Data = "Password changed successfully"
			});
		}



		private static string GenerateJwtToken(IdentityUser user)
		{
			var claims = new[]
			{
			new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			new Claim(ClaimTypes.NameIdentifier, user.Id)
		};



			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")!));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: Environment.GetEnvironmentVariable("JWT_ISSUER"),
				audience: Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
				claims: claims,
				expires: DateTime.Now.AddMinutes(Convert.ToDouble(Environment.GetEnvironmentVariable("JWT_EXPIRES"))),
				signingCredentials: creds);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		private static string GenerateRefreshToken()
		{
			var randomNumber = new byte[32];
			using var rng = RandomNumberGenerator.Create();
			rng.GetBytes(randomNumber);
			return Convert.ToBase64String(randomNumber);
		}

		private static ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
		{
			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")!)),
				ValidateIssuer = false,
				ValidateAudience = false,
				ValidateLifetime = false,
				ClockSkew = TimeSpan.Zero
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

			if ((securityToken is not JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
			{
				throw new SecurityTokenException("Invalid token");
			}

			return principal;
		}

		private static string GenerateVerificationCode()
		{
			var random = new Random();
			return random.Next(100000, 999999).ToString();
		}
	}
}
