using System.Drawing;
using VisionTest.Core.Models;
using VisionTest.Core.Recognition;
using VisionTest.Core.Services.Storage;

//add C:\Users\guill\Programmation\dotNET_doc\VisionTest\VisionTest.TestsImplementation buttonlik2 C:\Users\guill\Programmation\dotNET_doc\VisionTest\VisionTest.Tests\images\cottonLike2.png
//add C:\Users\guill\Programmation\dotNET_doc\VisionTest\VisionTest.TestsImplementation buttonlik C:\Users\guill\Programmation\dotNET_doc\VisionTest\VisionTest.Tests\images\cottonLike.png
//ocr C:\Users\guill\Programmation\dotNET_doc\VisionTest\VisionTest.Tests\images\cottonLike.png
//update C:\Users\guill\Programmation\dotNET_doc\VisionTest\VisionTest.TestsImplementation

namespace VisionTest.ConsoleInterop
{
    class Program
    {
        static async Task Main()
        {
            try
            {
                while (true)
                {
                    var stringLine = Console.ReadLine();
                    string[] commandLine = stringLine?.Split() ?? []; // Wait for user input to continue

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
                        case "remove":
                            await Remove(commandLine);
                            break;
                        default:
                            Console.WriteLine($"Unknown command: {commandLine[0]}");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Reads an image file and performs OCR to extract text from it
        /// </summary>
        /// <param name="args"></param>
        private static void ProcessOCRCommand(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage:ocr <imagePath>");
                return;
            }

            string imagePath = args[1];

            // Validate the image path
            if (!File.Exists(imagePath))
            {
                Console.WriteLine($"Error: The file '{imagePath}' does not exist.");
                return;
            }


            string extractedText;
            // Load the image
            using (Bitmap image = new Bitmap(imagePath))
            {
                // Initialize the OCR engine
                OCREngine ocrEngine = new OCREngine("eng");

                // Perform OCR to extract text
                extractedText = ocrEngine.GetText(image).TrimEnd('\n');
            }
            // Output the extracted text
            Console.WriteLine("Extracted Text:");
            Console.WriteLine(extractedText);

        }

        private static async Task AddNewElement(string[] args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("Usage: add <projectDirectory> <id> <imagePath>");
                return;
            }
            string projectDirectory = args[1];
            string id = args[2];
            string imagePath = args[3];


            var repositoryManager = new RepositoryManager(projectDirectory);
            var screenElement = new ScreenElement
            {
                Id = id,
                Images = { new Bitmap(imagePath) }
            };

            if ((await repositoryManager.GetAllScreenElementNamesAsync()).Contains(id))
                Console.WriteLine($"Screen element with ID '{id}' already exists. Please choose a different ID.");

            await repositoryManager.AddAsync(screenElement);
        }

        private static async Task Update(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: update <projectDirectory>");
                return;
            }

            var projectDirectory = args[1];


            var repositoryManager = new RepositoryManager(projectDirectory);

            await repositoryManager.UpdateEnumAsync(projectDirectory);
        }

        private static async Task Remove(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: remove <projectDirectory> <id>");
                return;
            }
            string projectDirectory = args[1];
            string id = args[2];

            var repositoryManager = new RepositoryManager(projectDirectory);
            await repositoryManager.RemoveElement(id);
        }
    }
}
