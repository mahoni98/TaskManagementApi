// api/Service/TokenService.cs
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.Interfaces;
using api.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace api.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _key;

        /// <summary>
        /// TokenService sınıfının yeni bir örneğini başlatır.
        /// </summary>
        /// <param name="configuration">Uygulama yapılandırma ayarlarına erişim sağlayan IConfiguration servisi.</param>
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"]));
        }

        /// <summary>
        /// Belirtilen kullanıcı için bir JWT (JSON Web Token) oluşturur.
        /// Token, kullanıcının e-posta adresini, kullanıcı adını ve ID'sini içerir.
        /// </summary>
        /// <param name="user">Token oluşturulacak olan AppUser nesnesi.</param>
        /// <returns>Oluşturulan JWT stringini döner.</returns>
        public string CreateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.UserName),
                // Kullanıcı ID'sini buraya ekliyoruz! Bu çok önemli.
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7), // Token geçerlilik süresi
                SigningCredentials = creds,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}