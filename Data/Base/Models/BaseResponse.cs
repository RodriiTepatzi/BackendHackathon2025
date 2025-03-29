namespace BackendHackathon2025.Data.Base.Models
{
	public class BaseResponse <T>
	{
		public T? Data { get; set; }
		public BaseError? Error { get; set; }
		public bool Ok => Error == null;

	}
}
