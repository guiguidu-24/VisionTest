using System.Drawing;
using System.Text.Json;
using VisionTest.Core.Models;
using VisionTest.Core.Recognition;
using VisionTest.Core.Services.Storage;

//add C:\Users\guill\Programmation\dotNET_doc\VisionTest\VisionTest.TestsImplementation buttonlik2 C:\Users\guill\Programmation\dotNET_doc\VisionTest\VisionTest.Tests\images\cottonLike2.png
//add C:\Users\guill\Programmation\dotNET_doc\VisionTest\VisionTest.TestsImplementation buttonlik C:\Users\guill\Programmation\dotNET_doc\VisionTest\VisionTest.Tests\images\cottonLike.png
//ocr C:\Users\guill\Programmation\dotNET_doc\VisionTest\VisionTest.Tests\images\cottonLike.png
//update C:\Users\guill\Programmation\dotNET_doc\VisionTest\VisionTest.TestsImplementation

namespace VisionTest.ConsoleInterop;

public class Program
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
    /// Reads an image file and performs OCR to extract text from it and prints a JSON response
    /// </summary>
    /// <param name="args"></param>
    public static void ProcessOCRCommand(string[] args)
    {
        // Error: Not enough arguments
        if (args.Length != 2)
        {
            var errorResponse = new
            {
                response = new
                {
                    status = "error",
                    message = $"{args.Length} arguments found, expected:2",
                    data = new
                    {
                        textFound = ""
                    }
                }
            };
            string errorJson = JsonSerializer.Serialize(errorResponse);
            Console.WriteLine(errorJson);
            return;
        }

        string imagePath = args[1];

        // Error: File does not exist
        if (!File.Exists(imagePath))
        {
            var errorResponse = new
            {
                response = new
                {
                    status = "error",
                    message = $"The file '{imagePath}' does not exist.",
                    data = new
                    {
                        textFound = ""
                    }
                }
            };
            string errorJson = JsonSerializer.Serialize(errorResponse);
            Console.WriteLine(errorJson);
            return;
        }

        string extractedText;
        // Load the image and perform OCR
        using (Bitmap image = new Bitmap(imagePath))
        {
            OCREngine ocrEngine = new OCREngine("eng");
            extractedText = ocrEngine.GetText(image).TrimEnd('\n');
        }

        // Success JSON response
        var response = new
        {
            response = new
            {
                status = "success",
                message = "All text read",
                data = new
                {
                    textFound = extractedText
                }
            }
        };

        string json = JsonSerializer.Serialize(response);
        Console.WriteLine(json);
    }

    public static async Task AddNewElement(string[] args)
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
            Images = { LoadSafelyImage(imagePath) }
        };

        if ((await repositoryManager.GetAllScreenElementNamesAsync()).Contains(id))
        {
            Console.WriteLine($"Screen element with ID '{id}' already exists. Please choose a different ID.");
        }
        else
        {
            await repositoryManager.AddAsync(screenElement);
        }

        if (args.Length == 5 && args[4] == "-delete")
        {
            File.Delete(imagePath);
        }
    }

    public static async Task Update(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: update <projectDirectory>");
            return;
        }

        var projectDirectory = args[1];


        var repositoryManager = new RepositoryManager(projectDirectory);

        await repositoryManager.UpdateIndexAsync();
    }

    public static async Task Remove(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("Usage: remove <projectDirectory> <id>");
            return;
        }
        string projectDirectory = args[1];
        string id = args[2];

        var repositoryManager = new RepositoryManager(projectDirectory);
        await repositoryManager.RemoveElementAsync(id);
    }

    public static Bitmap LoadSafelyImage(string imagePath)
    {
        Bitmap bmp;
        using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            bmp = new Bitmap(stream);
        }
        return bmp;
    }
}