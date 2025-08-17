using System.CommandLine;
using System.Drawing;
using System.Text.Json;
using System.Text.RegularExpressions;
using VisionTest.ConsoleInterop.Storage;
using VisionTest.Core.Models;
using VisionTest.Core.Recognition;

//add C:\Users\guill\Programmation\dotNET_doc\VisionTest\VisionTest.TestsImplementation buttonlik2 C:\Users\guill\Programmation\dotNET_doc\VisionTest\VisionTest.Tests\images\cottonLike2.png
//add C:\Users\guill\Programmation\dotNET_doc\VisionTest\VisionTest.TestsImplementation buttonlik C:\Users\guill\Programmation\dotNET_doc\VisionTest\VisionTest.Tests\images\cottonLike.png
//ocr C:\Users\guill\Programmation\dotNET_doc\VisionTest\VisionTest.Tests\images\cottonLike.png
//update C:\Users\guill\Programmation\dotNET_doc\VisionTest\VisionTest.TestsImplementation

namespace VisionTest.ConsoleInterop;

public class Program
{
    public static async Task Main()
    {
        // 1) Build our commands once
        var root = BuildRootCommand();

        try
        {
            // 2) Start interactive REPL
            while (true)
            {
                var line = Console.ReadLine();
                if (line == null) break;              // CTRL+D / EOF
                var tokens = SplitArguments(line);
                if (tokens.Length == 0) continue;     // empty line

                // 3) Try parse
                var result = root.Parse(tokens);
                if (result.Errors.Any())
                {
                    // Unknown command or bad args → same JSON error as before
                    WriteJson(new
                    {
                        status = "error",
                        message = $"Unknown command: {tokens[0]}"
                    });
                    continue;
                }

                // 4) Invoke the handler we wired up
                await result.InvokeAsync();
            }
        }
        catch (Exception ex)
        {
            // Fatal—same behavior as before
            WriteJson(new
            {
                status = "error",
                message = ex.Message
            });
            throw;
        }
    }

    private static RootCommand BuildRootCommand()
    {
        var root = new RootCommand();

        // shared args/options
        var imgPathArg = new Argument<string>("imagePath");
        var idArg = new Argument<string>("id");
        var projectDirArg = new Argument<string>("projectDirectory");
        var deleteOption = new Option<bool>("-d", "--delete")
        {
            Description = "Delete source image after add"
        };

        // ----- ocr <imagePath>
        var ocr = new Command("ocr", "Perform OCR on an image");
        ocr.Add(imgPathArg);
        ocr.SetAction(parseResult =>
        {
            ProcessOCRCommand(parseResult.GetValue(imgPathArg));
        });
        root.Add(ocr);

        // ----- add <projectDir> <id> <imagePath> [-d]
        var addCommand = new Command("add", "Add a new screen element");
        addCommand.Add(projectDirArg);
        addCommand.Add(idArg);
        addCommand.Add(imgPathArg);
        addCommand.Add(deleteOption);
        addCommand.SetAction(async parseResult =>
        {
            await AddNewElement(
                parseResult.GetValue(projectDirArg),
                parseResult.GetValue(idArg),
                parseResult.GetValue(imgPathArg),
                parseResult.GetValue(deleteOption));
        });
        root.Add(addCommand);

        // ----- update <projectDirectory>
        var update = new Command("update", "Rebuild the ScreenElements index");
        update.Add(projectDirArg);
        update.SetAction(async parseResult =>
        {
            await Update(
                parseResult.GetValue(projectDirArg));
        });
        root.Add(update);

        // ----- remove <projectDirectory> <id>
        var remove = new Command("remove", "Remove a screen element");
        remove.Add(projectDirArg);
        remove.Add(idArg);
        remove.SetAction(async parseResult =>
        {
            await Remove(
                parseResult.GetValue(projectDirArg),
                parseResult.GetValue(idArg));
        });
        root.Add(remove);

        return root;
    }


    // Helper to keep your existing JSON-in/JSON-out style
    private static void WriteJson(object payload)
    {
        Console.WriteLine(JsonSerializer.Serialize(new { response = payload }));
    }

    /// <summary>
    /// Splits a command-line string into an array, honoring quotes.
    /// E.g. input:  greet "John Doe"
    ///       output: ["greet", "John Doe"]
    /// </summary>
    internal static string[] SplitArguments(string commandLine)
    {
        var pattern = @"\G[ ]*(?:(['""])(.*?)\1|([^ '""]+))";
        var matches = Regex.Matches(commandLine, pattern);
        return matches
            .Cast<Match>()
            .Select(m => m.Groups[2].Success
                ? m.Groups[2].Value
                : m.Groups[3].Value)
            .ToArray();
    }


    public static void ProcessOCRCommand(string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
        {
            WriteJson(new { status = "error", message = "Image path cannot be empty." });
            return;
        }

        if (!File.Exists(imagePath))
        {
            WriteJson(new { status = "error", message = $"The file {imagePath} does not exist." });
            return;
        }

        string extractedText;
        using (var image = new Bitmap(imagePath))
        {
            var ocrEngine = new OCREngine("eng");
            extractedText = ocrEngine.GetText(image).TrimEnd('\n');
        }

        WriteJson(new
        {
            status = "success",
            message = "All text read",
            data = new { textFound = extractedText }
        });
    }

    public static async Task AddNewElement(string? projectDirectory, string? id, string? imagePath, bool deleteImage)
    {
        if (string.IsNullOrWhiteSpace(projectDirectory))
        {
            WriteJson(new { status = "error", message = "Project directory cannot be empty." });
            return;
        }

        if (string.IsNullOrWhiteSpace(id))
        {
            WriteJson(new { status = "error", message = "ID cannot be empty." });
            return;
        }

        if (string.IsNullOrWhiteSpace(imagePath))
        {
            WriteJson(new { status = "error", message = "Image path cannot be empty." });
            return;
        }

        if (!StringValidation.IsValidCSharpIdentifier(Path.GetFileNameWithoutExtension(id)))
        {
            WriteJson(new { status = "error", message = "Id is not valid" });
            return;
        }

        var repo = new RepositoryManager(projectDirectory);
        if ((repo.GetAllScreenElementNames()).Contains(id))
        {
            WriteJson(new { status = "error", message = $"Screen element with ID {id} already exists." });
        }
        else
        {
            var element = new ScreenElement { Id = id };
            using var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            element.Images.Add(new Bitmap(fs));
            await repo.AddAsync(element);
            WriteJson(new { status = "success", message = $"Screen element {id} added successfully." });
        }

        if (deleteImage)
            File.Delete(imagePath);
    }

    public static async Task Update(string? projectDirectory)
    {
        if (string.IsNullOrWhiteSpace(projectDirectory))
        {
            WriteJson(new { status = "error", message = "Project directory cannot be empty." });
            return;
        }

        var repo = new RepositoryManager(projectDirectory);
        await repo.UpdateIndexAsync();
        WriteJson(new { status = "success", message = $"Index updated successfully for {projectDirectory.Replace('\\', '/')}." });
    }

    public static async Task Remove(string? projectDirectory, string? id)
    {
        if (string.IsNullOrWhiteSpace(projectDirectory))
        {
            WriteJson(new { status = "error", message = "Project directory cannot be empty." });
            return;
        }

        if (string.IsNullOrWhiteSpace(id))
        {
            WriteJson(new { status = "error", message = "ID cannot be empty." });
            return;
        }

        var repo = new RepositoryManager(projectDirectory);
        var all = repo.GetAllScreenElementNames();
        if (!all.Contains(id))
        {
            WriteJson(new { status = "error", message = $"Screen element with ID {id} does not exist." });
        }
        else
        {
            await repo.RemoveElementAsync(id);
            WriteJson(new { status = "success", message = $"Screen element {id} removed successfully." });
        }
    }
}
