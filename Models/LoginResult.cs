using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace Models
{
	public class LoginResult
	{
		private JwtSecurityToken Token { get; }

		[OpenApiProperty(Description = "The access token to be used in every subsequent operation for this user.")]
		public string AccessToken => new JwtSecurityTokenHandler().WriteToken(Token);

		[OpenApiProperty(Description = "The token type.")]
		public string TokenType => "Bearer";

		[OpenApiProperty(Description = "The result description.")]
		public string Description { get; set; }

		[OpenApiProperty(Description = "The amount of seconds until the token expires.")]
		[JsonRequired]
		public int ExpiresIn => (int)(Token.ValidTo - DateTime.UtcNow).TotalSeconds;

		public LoginResult(JwtSecurityToken Token)
		{
			this.Token = Token;
		}

		public LoginResult()
		{
			this.Token = new JwtSecurityToken();
		}
	}

}
