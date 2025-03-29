using BackendHackathon2025.Data.Base.Models;

namespace BackendHackathon2025.Data.Const
{
	public class ResponseErrors
	{
		public static BaseError AuthDuplicatedUser = new BaseError { Code = "AU001", Message = "User already exists" };
		public static BaseError AuthInvalidData = new BaseError { Code = "AU002", Message = "Invalid data" };
		public static BaseError AuthInvalidCredentials = new BaseError { Code = "AU003", Message = "Invalid credentials" };
		public static BaseError AuthErrorCreatingUser = new BaseError { Code = "AU004", Message = "Error creating user" };
		public static BaseError AuthUserEmailAlreadyExists = new BaseError { Code = "AU005", Message = "User email already registered" };
		public static BaseError AuthUserExpedienteAlreadyExists = new BaseError { Code = "AU006", Message = "User identification number already registered" };
		public static BaseError AuthInvalidToken = new BaseError { Code = "AU007", Message = "Invalid token" };
		public static BaseError AuthInvalidRefreshToken = new BaseError { Code = "AU008", Message = "Invalid refresh token" };
		public static BaseError AuthUserNotFound = new BaseError { Code = "AU009", Message = "User not found" };
		public static BaseError AuthErrorUpdatingUser = new BaseError { Code = "AU010", Message = "Error updating user" };
		public static BaseError AuthInvalidCurrentPassword = new BaseError { Code = "AU011", Message = "Invalid current password" };
		public static BaseError AuthErrorChangingPassword = new BaseError { Code = "AU012", Message = "Error changing password" };
		public static BaseError AuthRefreshTokenExpired = new BaseError { Code = "AU013", Message = "Refresh token expired" };
		public static BaseError AuthPhoneNumberAlreadyExists = new BaseError { Code = "AU014", Message = "Phone number already registered." };
		public static BaseError AuthInvalidVerificationCode = new BaseError { Code = "AU015", Message = "Verification code is incorrect." };
		public static BaseError AuthVerificationCodeExpired = new BaseError { Code = "AU016", Message = "Verification code is expired." };
		public static BaseError AuthTooManyTries = new BaseError { Code = "AU017", Message = "Too many tries. Try again later." };

	}
}
