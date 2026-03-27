using System.Security.Cryptography;
using System.Text;

namespace EngenhariasSenac.Helpers;
public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        // Gerar um salt aleatório de 16 bytes
        byte[] salt = RandomNumberGenerator.GetBytes(16);

        // Derivar o hash usando PBKDF2 com 100.000 iterações
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(32); // Tamanho do hash

        // Concatenar salt + hash e converter em Base64 para armazenar
        byte[] hashBytes = new byte[48]; // 16 salt + 32 hash
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 32);

        return Convert.ToBase64String(hashBytes);
    }

    public static bool VerifyPassword(string password, string storedHash)
    {
        byte[] hashBytes = Convert.FromBase64String(storedHash);

        byte[] salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);

        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(32);

        for (int i = 0; i < 32; i++)
        {
            if (hashBytes[i + 16] != hash[i])
                return false;
        }

        return true;
    }
}
