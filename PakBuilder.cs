﻿namespace ChunkMaker
{
    public class PakBuilder
    {
        public Dictionary<string, List<string>> CreateChunksFromFolder(string gameFolder, string chunkFolder, int chunkSizeInBytes, string hashFilePath)
        {
            if (!Directory.Exists(chunkFolder))
            {
                Directory.CreateDirectory(chunkFolder);
            }

            var previousHashes = FileHashManager.LoadHashesFromFile(hashFilePath);
            var currentHashes = FileHashManager.CalculateHashesForGameFolder(gameFolder);

            var filesToUpdate = GetChangedFiles(previousHashes, currentHashes, gameFolder);
            var pakFilesDictionary = new Dictionary<string, List<string>>(); // Nový slovník, kde budeme ukládat hash souboru a chunkové soubory

            foreach (var file in filesToUpdate)
            {
                // Vypočítáme hash původního souboru
                string originalFileHash = FileHashManager.GetFileHash(file);

                // Vytvoříme chunkové soubory a přidáme je pod příslušný hash do slovníku
                pakFilesDictionary[originalFileHash] = CreateChunksFromFile(file, gameFolder, chunkFolder, chunkSizeInBytes);
            }

            // Uložíme aktuální hashe do hash souboru
            FileHashManager.SaveHashesToFile(hashFilePath, currentHashes);

            return pakFilesDictionary; // Vrátíme slovník, který obsahuje hash původního souboru a jeho chunkové soubory
        }


        private List<string> GetChangedFiles(Dictionary<string, string> previousHashes, Dictionary<string, string> currentHashes, string gameFolder)
        {
            if (previousHashes == null)
            {
                Console.WriteLine("previousHashes is null");
            }

            if (currentHashes == null)
            {
                Console.WriteLine("currentHashes is null");
            }

            var filesToUpdate = new List<string>();

            foreach (var file in currentHashes)
            {
                if (!previousHashes.ContainsKey(file.Key) || previousHashes[file.Key] != file.Value)
                {
                    filesToUpdate.Add(Path.Combine(gameFolder, file.Key)); // Add full path to the file
                }
            }

            return filesToUpdate;
        }

        private List<string> CreateChunksFromFile(string filePath, string gameFolder, string chunkFolder, int chunkSizeInBytes)
        {
            byte[] buffer = new byte[chunkSizeInBytes];
            int chunkIndex = 0;
            var createdChunks = new List<string>(); // List to hold created .pak files

            // Create a file path relative to the game folder
            string relativePath = Path.GetRelativePath(gameFolder, filePath);
            string chunkFilePrefix = Path.Combine(chunkFolder, relativePath.Replace(Path.DirectorySeparatorChar, '_'));

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                int bytesRead;
                while ((bytesRead = fileStream.Read(buffer, 0, chunkSizeInBytes)) > 0)
                {
                    string chunkFilePath = $"{chunkFilePrefix}_part{chunkIndex + 1}.pak";
                    Console.WriteLine($"Processing file: {relativePath}");
                    
                    // Write the chunk to a file
                    using (var chunkStream = new FileStream(chunkFilePath, FileMode.Create, FileAccess.Write))
                    {
                        chunkStream.Write(buffer, 0, bytesRead);
                    }

                    // Add the created chunk file path to the list
                    createdChunks.Add(chunkFilePath);
                    chunkIndex++;
                }
            }

            return createdChunks; // Return the list of .pak files created for this file
        }
    }
}
