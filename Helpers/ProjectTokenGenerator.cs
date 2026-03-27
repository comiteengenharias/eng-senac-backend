using System.Text;
using System;

namespace EngenhariasSenac.Helpers;

public class ProjectTokenGenerator
{
    private static readonly Random random = new Random();

    public static string Generate()
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < 3; i++)
        {
            // Adiciona uma letra maiúscula (A-Z)
            char letter = (char)random.Next('A', 'Z' + 1);
            sb.Append(letter);

            // Adiciona um número (0-9)
            int digit = random.Next(0, 10);
            sb.Append(digit);
        }

        return sb.ToString(); // Ex: "A1C7D3"
    }
}
