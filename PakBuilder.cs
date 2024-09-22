namespace ChunkMaker;

public class PakBuilder
    {
        public void CreateChunksFromFolder(string gameFolder, string chunkFolder, int chunkSizeInBytes, string hashFilePath)
        {
            if (!Directory.Exists(chunkFolder))
            {
                Directory.CreateDirectory(chunkFolder);
            }

            var previousHashes = FileHashManager.LoadHashesFromFile(hashFilePath);

            var currentHashes = FileHashManager.CalculateHashesForGameFolder(gameFolder);

            var filesToUpdate = GetChangedFiles(previousHashes, currentHashes, gameFolder);

            foreach (var file in filesToUpdate)
            {
                CreateChunksFromFile(file, gameFolder, chunkFolder, chunkSizeInBytes);
            }

            FileHashManager.SaveHashesToFile(hashFilePath, currentHashes);
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
                    filesToUpdate.Add(Path.Combine(gameFolder, file.Key)); // Returns the full path to the file
                }
            }

            return filesToUpdate;
        }
        
        private void CreateChunksFromFile(string filePath, string gameFolder, string chunkFolder, int chunkSizeInBytes)
        {
            byte[] buffer = new byte[chunkSizeInBytes];
            int chunkIndex = 0;

            // Create a file path relative to the game folder
            string relativePath = Path.GetRelativePath(gameFolder, filePath);
            string chunkFilePrefix = Path.Combine(chunkFolder, relativePath.Replace(Path.DirectorySeparatorChar, '_'));

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                int bytesRead;
                while ((bytesRead = fileStream.Read(buffer, 0, chunkSizeInBytes)) > 0)
                {
                    string chunkFilePath = $"{chunkFilePrefix}_part{chunkIndex + 1}.pak";
                    Console.WriteLine($" Processing file: {relativePath}");
                    using (var chunkStream = new FileStream(chunkFilePath, FileMode.Create, FileAccess.Write))
                    {
                        chunkStream.Write(buffer, 0, bytesRead);
                    }

                    chunkIndex++;
                }
            }
        }
    }