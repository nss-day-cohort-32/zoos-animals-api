using AnimalsAPI.Interfaces;
using AnimalsAPI.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AnimalsAPI.Services
{
    public class TokenAuthenticationService : IAuthenticateService
    {
        private readonly IUserManagementService _userManagementService;
        private readonly Token _token;

        public TokenAuthenticationService(IUserManagementService service, IOptions<Token> tokenManagement)
        {
            _userManagementService = service;
            _token = tokenManagement.Value;
        }
        public bool IsAuthenticated(string connectionString, AuthenticationRequest request, out string token)
        {

            token = string.Empty;
            if (!_userManagementService.IsValidUser(connectionString, request.Username, request.Password)) return false;

            var claim = new[]
            {
                new Claim(ClaimTypes.Name, request.Username)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_token.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                _token.Issuer,
                _token.Audience,
                claim,
                expires: DateTime.Now.AddMinutes(_token.AccessExpiration),
                signingCredentials: credentials
            );
            token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return true;

        }
    }

}
