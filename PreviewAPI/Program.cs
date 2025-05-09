using System;
using System.Drawing;
using System.IO;
using POC_Tesseract;

namespace PreviewAPI
{
    class Program
    {
        static void Main(string[] args)
        {
           
            while (true)
            {
                string? input = Console.ReadLine(); // Allow input to be nullable  

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Invalid command. Please try again.");
                    continue;
                }

                string[] commandParts = input.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                string command = commandParts[0].ToLower();

                switch (command)
                {
                    case "ocr":
                        if (commandParts.Length < 2)
                        {
                            Console.WriteLine("Usage: ocr <imagePath>");
                        }
                        else
                        {
                            string imagePath = commandParts[1];
                            ProcessOCRCommand(imagePath);
                        }
                        break;

                    case "exit":
                        Console.WriteLine("Exiting the application. Goodbye!");
                        return;

                    default:
                        Console.WriteLine($"Unknown command: {command}");
                        break;
                }
            }
        }

        private static void ProcessOCRCommand(string imagePath)
        {
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
    }
}
