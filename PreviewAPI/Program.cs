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
            // Check if arguments are provided
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: previewAPI.exe ocr <imagePath>");
                return;
            }

            // Parse the command and arguments
            string command = args[0].ToLower();
            string imagePath = args[1];

            switch (command)
            {
                case "ocr":
                    ProcessOCRCommand(imagePath);
                    break;

                default:
                    Console.WriteLine($"Unknown command: {command}");
                    Console.WriteLine("Usage: previewAPI.exe ocr <imagePath>");
                    break;
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
