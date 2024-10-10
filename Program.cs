using Newtonsoft.Json;

namespace ChunkMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            string gameFolder;
            string outputFolder;
            string hashFolder;
            int partSizeInBytes = 10 * 1024 * 1024; // Velikost chunku (10 MB)
            string hashFilePath;
            string pakFileListPath;

            if (args.Length >= 2)
            {
                gameFolder = args[0];
                outputFolder = args[1];
                hashFolder = args[2];
            }
            else
            {
                Console.Write("Choose folder with a game: ");
                gameFolder = Console.ReadLine();

                Console.Write("Choose destination folder: ");
                outputFolder = Console.ReadLine();
                
                Console.Write("Choose folder for Hash.json file: ");
                hashFolder = Console.ReadLine();
            }

            if (!Directory.Exists(gameFolder))
            {
                Console.WriteLine($"Game Folder '{gameFolder}' doesn't exist.");
                return;
            }

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            if (!Directory.Exists(hashFolder))
            {
                Directory.CreateDirectory(hashFolder);
            }

            hashFilePath = Path.Combine(hashFolder, "Hash.json");
            pakFileListPath = Path.Combine(hashFolder, "PakFiles.json"); // Cesta k seznamu .pak souborů
            
            Console.WriteLine(" ");
            Console.WriteLine($"Game folder: {gameFolder}");
            Console.WriteLine($"Output folder: {outputFolder}");
            Console.WriteLine($"Folder for hash: {hashFolder}");
            Console.WriteLine(" ");
            Console.WriteLine("Starting splitting into chunks...");

            // Processing of all files in folder
            var pakBuilder = new PakBuilder();
            var pakFiles = pakBuilder.CreateChunksFromFolder(gameFolder, outputFolder, partSizeInBytes, hashFilePath);

            // Uložení seznamu .pak souborů do JSON souboru
            SavePakFileListToJson(pakFiles, pakFileListPath);

            Console.WriteLine("Done");
        }

        // Funkce pro uložení seznamu .pak souborů do JSON
        static void SavePakFileListToJson(Dictionary<string, List<string>> pakFiles, string pakFileListPath)
        {
            var pakFileDictionary = new Dictionary<string, Dictionary<string, string>>();

            foreach (var originalFileHash in pakFiles.Keys)
            {
                // Vytvoříme nový vnořený slovník, kde název chunku je klíčem a jeho hash je hodnotou
                var chunkFiles = new Dictionary<string, string>();

                foreach (var pakFile in pakFiles[originalFileHash])
                {
                    string chunkHash = FileHashManager.GetFileHash(pakFile); // Vypočítáme hash chunkového souboru
                    string fileName = Path.GetFileName(pakFile); // Získáme název chunkového souboru
                    chunkFiles[fileName] = chunkHash;
                }

                pakFileDictionary[originalFileHash] = chunkFiles;
            }

            // Serializace do JSON a zápis do souboru
            string json = JsonConvert.SerializeObject(pakFileDictionary, Formatting.Indented);
            File.WriteAllText(pakFileListPath, json);

            Console.WriteLine($"Pak file list saved to: {pakFileListPath}");
        }
    }
}
