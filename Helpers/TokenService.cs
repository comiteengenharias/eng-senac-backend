using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;
using EngenhariasSenac.Banco;
using EngenhariasSenac.Models;
using EngenhariasSenac.Database;

public class TokenService
{
    public static (string accessToken, string refreshToken) GenerateTokens(string email, string role, int userId)
    {
        var codeJwt = Environment.GetEnvironmentVariable("CODE_JWT") ?? throw new Exception("Código JWT não definido");
        var key = Encoding.ASCII.GetBytes(codeJwt);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, role)
        };

        // Adiciona o ID específico baseado na role
        if (role == "Student")
            claims.Add(new Claim("CodStudent", userId.ToString()));
        else if (role == "Teacher")
            claims.Add(new Claim("CodTeacher", userId.ToString()));
        else if (role == "Support")
            claims.Add(new Claim("CodSupport", userId.ToString()));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(token);

        var refreshTokenString = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var context = new EngenhariasSenacContext();
        var dalToken = new DAL<RefreshToken>(context);
        dalToken.Insert(new RefreshToken
        {
            Token = refreshTokenString,
            Username = email,
            ExpirationDate = DateTime.UtcNow.AddDays(1)
        });

        return (accessToken, refreshTokenString);
    }

    public static ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var codeJwt = Environment.GetEnvironmentVariable("CODE_JWT") ?? throw new Exception("Código JWT não definido");
            var key = Encoding.ASCII.GetBytes(codeJwt);

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false, // Configure se quiser validar
                ValidateAudience = false, // Configure se quiser validar
                ClockSkew = TimeSpan.Zero // Elimina o tempo de tolerância
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            // Garante que o token seja um JWT e use HmacSha256
            if (validatedToken is JwtSecurityToken jwtToken &&
                jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return principal;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

}
