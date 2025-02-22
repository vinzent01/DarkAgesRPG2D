using Raylib_cs;

public class Utils{

    public static Color HexToColor(string hex)
    {
        if (string.IsNullOrWhiteSpace(hex))
            throw new ArgumentException("A string hexadecimal não pode ser nula ou vazia.");

        // Remove o # se ele estiver presente
        hex = hex.TrimStart('#');

        // Verifica se o comprimento é válido (6 ou 8 caracteres)
        if (hex.Length != 6 && hex.Length != 8)
            throw new ArgumentException("A string hexadecimal deve ter 6 ou 8 caracteres.");

        // Converte os componentes
        byte r = Convert.ToByte(hex.Substring(0, 2), 16);
        byte g = Convert.ToByte(hex.Substring(2, 2), 16);
        byte b = Convert.ToByte(hex.Substring(4, 2), 16);
        byte a = hex.Length == 8 ? Convert.ToByte(hex.Substring(6, 2), 16) : (byte)255;

        return new Color(r, g, b, a);
    }
}