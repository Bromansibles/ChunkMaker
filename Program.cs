namespace ChunkMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            string gameFolder;
            string outputFolder;
            string hashFolder;
            int partSizeInBytes = 10 * 1024 * 1024;
            string hashFilePath;

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
            
            Console.WriteLine(" ");
            Console.WriteLine($"Game folder: {gameFolder}");
            Console.WriteLine($"Output folder: {outputFolder}");
            Console.WriteLine($"Folder for hash: {hashFolder}");
            Console.WriteLine(" ");
            Console.WriteLine("Starting splitting into chunks...");

            // Processing of all files in folder
            var pakBuilder = new PakBuilder();
            pakBuilder.CreateChunksFromFolder(gameFolder, outputFolder, partSizeInBytes, hashFilePath);

            Console.WriteLine("Done");
        }
    }
}