namespace ChunkMaker;

using System.Security.Cryptography;
using Newtonsoft.Json;

public class FileHashManager
{
    public static Dictionary<string, string> CalculateHashesForGameFolder(string gameFolder)
    {
        var fileHashes = new Dictionary<string, string>();
        var files = Directory.GetFiles(gameFolder, "*.*", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            string relativePath = Path.GetRelativePath(gameFolder, file);
            string fileHash = GetFileHash(file);
            fileHashes[relativePath] = fileHash;
        }

        return fileHashes;
    }


    public static string GetFileHash(string filePath)
    {
        using (var sha256 = SHA256.Create())
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] hashBytes = sha256.ComputeHash(fileStream);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }

    public static void SaveHashesToFile(string filePath, Dictionary<string, string> fileHashes)
    {
        string json = JsonConvert.SerializeObject(fileHashes, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }

    public static Dictionary<string, string> LoadHashesFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return new Dictionary<string, string>();
        }

        string json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
    }

}