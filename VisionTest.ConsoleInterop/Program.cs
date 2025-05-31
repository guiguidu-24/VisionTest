using System.Drawing;
using VisionTest.Core.Models;
using VisionTest.Core.Recognition;
using VisionTest.Core.Services;

namespace VisionTest.ConsoleInterop
{
    class Program
    {
        static async Task Main(string[] args)
        {

            // Parse the command and arguments
            string command = args[0].ToLower();

            switch (command)
            {
                case "ocr":
                    string imagePath = args[1];
                    ProcessOCRCommand(args);
                    break;
                case "add":
                    await AddNewElement(args);
                    break;

                default:
                    Console.WriteLine($"Unknown command: {command}");
                    Console.WriteLine("Usage: runner.exe ocr <imagePath>");
                    break;
            }
        }

        private static void ProcessOCRCommand(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: runner.exe ocr <imagePath>");
                return;
            }

            string imagePath = args[1];

            // Validate the image path
            if (!File.Exists(imagePath))
            {
                Console.WriteLine($"Error: The file '{imagePath}' does not exist.");
                return;
            }

            try
            {
                // Load the image
                using Bitmap image = new Bitmap(imagePath);

                // Initialize the OCR engine
                OCREngine ocrEngine = new OCREngine("eng");

                // Perform OCR to extract text
                string extractedText = ocrEngine.GetText(image);

                // Output the extracted text
                Console.WriteLine("Extracted Text:");
                Console.WriteLine(extractedText);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static async Task AddNewElement(string[] args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("Usage: runner.exe add <projectDirectory> <id> <imagePath>");
                return;
            }

            string projectDirectory = args[1];
            string id = args[2];
            string imagePath = args[3];

            try
            {
                var repositoryManager = new RepositoryManager(projectDirectory);
                var screenElement = new ScreenElement
                {
                    Id = id,
                    Images = { new Bitmap(imagePath) }
                };

                await repositoryManager.AddAsync(screenElement, projectDirectory);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
