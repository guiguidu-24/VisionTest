using System;
using System.Drawing;
using System.Windows.Shapes;
using VisionTest.Core.Models;
using VisionTest.Core.Recognition;
using VisionTest.Core.Services;
using VisionTest.Core.Services.Storage;

namespace VisionTest.ConsoleInterop
{
    class Program
    {
        static async Task Main()
        {
            
            while(true)
            {
                string[] commandLine  = Console.ReadLine()?.Split() ?? []; // Wait for user input to continue

                switch (commandLine[0])
                {
                    case "ocr":
                        string imagePath = commandLine[1];
                        ProcessOCRCommand(commandLine);
                        break;
                    case "add":
                        await AddNewElement(commandLine);
                        break;
                    case "update":
                        await Update(commandLine); //TODO: Fire and forget
                        break;
                    default:
                        Console.WriteLine($"Unknown command: {commandLine[0]}");
                        break;
                }
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
                throw;
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

                await repositoryManager.AddAsync(screenElement);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        
        private static async Task Update(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: runner.exe update <projectDirectory>");
                return;
            }

            var projectDirectory = args[1];

            try
            {
                var repositoryManager = new RepositoryManager(projectDirectory);

                await repositoryManager.UpdateEnumAsync(projectDirectory);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }


        }
    }
}
