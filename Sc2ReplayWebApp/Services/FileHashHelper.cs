using System.Security.Cryptography;

public static class FileHashHelper
{
    /// <summary>
    /// Generates a SHA256 hash for the given file path.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string ComputeSha256(string filePath)
    {
        using var stream = File.OpenRead(filePath);

        using var sha256 = SHA256.Create();

        var hash = sha256.ComputeHash(stream);

        return Convert.ToHexString(hash);
    }
}