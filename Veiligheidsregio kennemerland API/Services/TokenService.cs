using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Kennemerland.Models;
using Kennemerland.DAL;
using Microsoft.OpenApi.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.IO;
using Newtonsoft.Json;
using System.Security.Claims;
using System;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Task = System.Threading.Tasks.Task;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Kennemerland.Services
{
    public interface ITokenService
    {
        Task<LoginResult> CreateToken(LoginRequest Login);
        Task<ClaimsPrincipal> GetByValue(string Value);
    }

    public class TokenService : ITokenService
    {
        public class TokenIdentityValidationParameters : TokenValidationParameters
        {
            public TokenIdentityValidationParameters(string Issuer, string Audience, SymmetricSecurityKey SecurityKey)
            {
                RequireSignedTokens = true;
                ValidAudience = Audience;
                ValidateAudience = true;
                ValidIssuer = Issuer;
                ValidateIssuer = true;
                ValidateIssuerSigningKey = true;
                ValidateLifetime = true;
                IssuerSigningKey = SecurityKey;
                AuthenticationType = JwtBearerDefaults.AuthenticationScheme;
            }
        }

        private ILogger Logger { get; }
        private string Issuer { get; }
        private string Audience { get; }
        private TimeSpan ValidityDuration { get; }
        private SigningCredentials Credentials { get; }
        private TokenIdentityValidationParameters ValidationParameters { get; }

        private readonly IUserRepository _users;

        public TokenService(IConfiguration Configuration, ILogger<TokenService> Logger, IUserRepository userRepository)
        {
            this.Logger = Logger;
            this._users = userRepository;
            Issuer = "DebugIssuer";                     // Configuration.GetClassValueChecked("JWT:Issuer", "DebugIssuer", Logger);
            Audience = "DebugAudience";                 // Configuration.GetClassValueChecked("JWT:Audience", "DebugAudience", Logger);
            ValidityDuration = TimeSpan.FromDays(1);    // Configure time length duration for valid key
            string Key = "DebugKey DebugKey";           // Configuration.GetClassValueChecked("JWT:Key", "DebugKey DebugKey", Logger);

            SymmetricSecurityKey SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));

            Credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256Signature);

            ValidationParameters = new TokenIdentityValidationParameters(Issuer, Audience, SecurityKey);
        }

        public async Task<LoginResult> CreateToken(LoginRequest Login)
        {
            BsonDocument user = _users.GetUser(Login.Username);

            User userToLogin = new User();
            if (user != null)
            {
                user.Remove("_id");
                userToLogin = BsonSerializer.Deserialize<User>(user);
                if (userToLogin.Password == Login.Password)
                {
                    JwtSecurityToken Token = await CreateToken(new Claim[] { new Claim(ClaimTypes.Role, (userToLogin.UserType).ToString()), new Claim(ClaimTypes.Name, Login.Username) });
                    return new LoginResult(Token);
                }
                else return new LoginResult { Description = "Login failed, please try again" };
            }
            else return new LoginResult { Description = "Login failed, please try again" };
        }

        private async Task<JwtSecurityToken> CreateToken(Claim[] Claims)
        {
            JwtHeader Header = new JwtHeader(Credentials);

            JwtPayload Payload = new JwtPayload(Issuer,
                           Audience,
                                                Claims,
                                                DateTime.UtcNow,
                                                DateTime.UtcNow.Add(ValidityDuration),
                                                DateTime.UtcNow);

            JwtSecurityToken SecurityToken = new JwtSecurityToken(Header, Payload);

            return await Task.FromResult(SecurityToken);
        }

        public async Task<ClaimsPrincipal> GetByValue(string Value)
        {
            if (Value == null)
            {
                throw new Exception("No Token supplied");
            }

            JwtSecurityTokenHandler Handler = new JwtSecurityTokenHandler();

            try
            {
                SecurityToken ValidatedToken;
                ClaimsPrincipal Principal = Handler.ValidateToken(Value, ValidationParameters, out ValidatedToken);

                return await Task.FromResult(Principal);
            }
            catch (Exception e)
            {
                throw;
            }
        }



    }
}
