
namespace FCG.Core
{
	public class JwtConfigurations
	{
		public required string Key { get; set; }
		public required string Issuer { get; set; }
		public required string Audience { get; set; }
		public required int Expiration { get; set; }
	}
}
