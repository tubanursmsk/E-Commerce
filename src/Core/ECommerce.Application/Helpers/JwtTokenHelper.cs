using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ECommerce.Application.Helpers
{
    public static class JwtTokenHelper
    {
        // Config değerleri (appsettings.json’dan alabilirsin)
        private static readonly string SecretKey = "MySuperSecretKeyForJwt123456!MySuperSecretKeyForJwt123456!"; // bu kısımda proje geliştime aşamasında old. için scretkey yazımı  statik ve görünür oldu  eğer daha güvenli hale getirmek istersek DI ile program.cs içindeki Jwt configrasyonu ile düzenleye biliriz 
        private static readonly string Issuer = "ECommerceApp"; //yayıncı
        private static readonly string Audience = "ECommerceUsers"; //izleyici

        // Token üret
        public static string GenerateToken(Guid userId, string username, Guid companyId, List<string> roles, int expireMinutes = 60)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim("companyId", companyId.ToString())
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Token doğrula
        public static ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(SecretKey);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Issuer,
                    ValidAudience = Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero // expiration toleransı
                }, out _);

                return principal;
            }
            catch
            {
                return null;
            }
        }

        // Token süresi dolmuş mu?
        public static bool IsExpired(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return jwt.ValidTo < DateTime.UtcNow;
        }
    }
}