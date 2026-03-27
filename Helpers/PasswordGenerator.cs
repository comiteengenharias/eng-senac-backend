namespace EngenhariasSenac.Helpers
{
    public static class PasswordGenerator
    {
        /// <summary>
        /// Gera uma senha aleatória segura com os caracteres especificados.
        /// </summary>
        /// <param name="length">Comprimento da senha desejada.</param>
        /// <returns>Senha aleatória.</returns>
        public static string Generate(int length = 8)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
