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
                        // Unknown command error in JSON format
                        var errorResponse = new
                        {
                            response = new
                            {
                                status = "error",
                                message = $"Unknown command: {commandLine[0]}"
                            }
                        };
                        string errorJson = JsonSerializer.Serialize(errorResponse);
                        Console.WriteLine(errorJson);
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            // Print any fatal error in JSON format
            var errorResponse = new
            {
                response = new
                {
                    status = "error",
                    message = ex.Message
                }
            };
            string errorJson = JsonSerializer.Serialize(errorResponse);
            Console.WriteLine(errorJson);
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
                    message = $"{args.Length} arguments found, expected:2"
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
                    message = $"The file '{imagePath}' does not exist."
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
        // Error: Not enough arguments
        if (args.Length < 4)
        {
            var errorResponse = new
            {
                response = new
                {
                    status = "error",
                    message = "Usage: add <projectDirectory> <id> <imagePath>"
                }
            };
            string errorJson = JsonSerializer.Serialize(errorResponse);
            Console.WriteLine(errorJson);
            return;
        }

        // Detect -d option in any position
        bool deleteImage = args.Any(a => a == "-d");

        // Filter out -d from arguments to get correct positions
        var filteredArgs = args.Where(a => a != "-d").ToArray();
        if (filteredArgs.Length < 4)
        {
            var errorResponse = new
            {
                response = new
                {
                    status = "error",
                    message = "Usage: add <projectDirectory> <id> <imagePath>"
                }
            };
            string errorJson = JsonSerializer.Serialize(errorResponse);
            Console.WriteLine(errorJson);
            return;
        }

        string projectDirectory = filteredArgs[1];
        string id = filteredArgs[2];
        if (string.IsNullOrWhiteSpace(id))
        {
            var errorResponse = new
            {
                response = new
                {
                    status = "error",
                    message = "ID cannot be empty."
                }
            };
            string errorJson = JsonSerializer.Serialize(errorResponse);
            Console.WriteLine(errorJson);
            return;
        }
        string imagePath = filteredArgs[3];

        var repositoryManager = new RepositoryManager(projectDirectory);
        var screenElement = new ScreenElement
        {
            Id = id,
            Images = { LoadSafelyImage(imagePath) }
        };

        if ((await repositoryManager.GetAllScreenElementNamesAsync()).Contains(id))
        {
            var errorResponse = new
            {
                response = new
                {
                    status = "error",
                    message = $"Screen element with ID '{id}' already exists. Please choose a different ID."
                }
            };
            string errorJson = JsonSerializer.Serialize(errorResponse);
            Console.WriteLine(errorJson);
        }
        else
        {
            await repositoryManager.AddAsync(screenElement);
            var successResponse = new
            {
                response = new
                {
                    status = "success",
                    message = $"Screen element '{id}' added successfully."
                }
            };
            string successJson = JsonSerializer.Serialize(successResponse);
            Console.WriteLine(successJson);
        }

        if (deleteImage)
        {
            File.Delete(imagePath);
        }
    }

    public static async Task Update(string[] args)
    {
        // Error: Not enough arguments
        if (args.Length < 2)
        {
            var errorResponse = new
            {
                response = new
                {
                    status = "error",
                    message = "Usage: update <projectDirectory>"
                }
            };
            string errorJson = JsonSerializer.Serialize(errorResponse);
            Console.WriteLine(errorJson);
            return;
        }

        string projectDirectory = args[1];

        var repositoryManager = new RepositoryManager(projectDirectory);
        await repositoryManager.UpdateIndexAsync();

        var successResponse = new
        {
            response = new
            {
                status = "success",
                message = $"Index updated successfully for project directory '{projectDirectory}'."
            }
        };
        string successJson = JsonSerializer.Serialize(successResponse);
        Console.WriteLine(successJson);
    }

    public static async Task Remove(string[] args)
    {
        // Error: Not enough arguments
        if (args.Length < 3)
        {
            var errorResponse = new
            {
                response = new
                {
                    status = "error",
                    message = "Usage: remove <projectDirectory> <id>"
                }
            };
            string errorJson = JsonSerializer.Serialize(errorResponse);
            Console.WriteLine(errorJson);
            return;
        }

        string projectDirectory = args[1];
        string id = args[2];

        var repositoryManager = new RepositoryManager(projectDirectory);

        // Check if the element exists before removing
        var allNames = await repositoryManager.GetAllScreenElementNamesAsync();
        if (!allNames.Contains(id))
        {
            var errorResponse = new
            {
                response = new
                {
                    status = "error",
                    message = $"Screen element with ID '{id}' does not exist in project directory '{projectDirectory}'."
                }
            };
            string errorJson = JsonSerializer.Serialize(errorResponse);
            Console.WriteLine(errorJson);
            return;
        }

        await repositoryManager.RemoveElementAsync(id);
        var successResponse = new
        {
            response = new
            {
                status = "success",
                message = $"Screen element '{id}' removed successfully from project directory '{projectDirectory}'."
            }
        };
        string successJson = JsonSerializer.Serialize(successResponse);
        Console.WriteLine(successJson);
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