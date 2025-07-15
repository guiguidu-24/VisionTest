using Microsoft.VisualStudio.Shell.Interop;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using VisionTest.VSExtension.Services;

namespace VisionTest.VSExtension
{
    public class InteropService
    {
        private readonly InteropProcess _interopProcess;

        public InteropService()
        {
            _interopProcess = new InteropProcess();
            _interopProcess.Start();
        }

        public async Task AddAsync(BitmapImage image, string id)
        {
            var tempImagePath = SaveBitmapImageToTemp(image);
            string command = $"add {ProjectService.GetActiveProjectDirectory()} {id} {tempImagePath} -d";
            _interopProcess.StandardInput.WriteLine(command);

            // Wait for the JSON response asynchronously
            string jsonResponse = await _interopProcess.StandardOutput.ReadLineAsync();
            // Clean up temp file if not already deleted
            if (File.Exists(tempImagePath))
            {
                try
                {
                    File.Delete(tempImagePath);
                }
                catch (IOException ex)
                {
                    Debug.WriteLine($"Failed to delete temporary file: {ex.Message}");
                    throw;
                }
            }

            // Parse and handle the JSON response
            if (string.IsNullOrWhiteSpace(jsonResponse))
                throw new InvalidOperationException("No response received from Add command.");

            try
            {
                var root = Newtonsoft.Json.Linq.JObject.Parse(jsonResponse);
                var response = root["response"];
                if (response == null)
                    throw new InvalidOperationException("Invalid Add response format: missing 'response' property.");

                var status = response["status"]?.Value<string>();
                var message = response["message"]?.Value<string>();

                if (status == "error")
                    throw new InvalidOperationException($"Add command returned error: {message}");

                if (status != "success")
                    throw new InvalidOperationException($"Unexpected status in Add response: {status}");

                // Success: nothing to return, but you could log message if needed
            }
            catch (Newtonsoft.Json.JsonException ex)
            {
                throw new InvalidOperationException("Failed to parse Add JSON response.", ex);
            }
        }

        public async Task UpdateEnumAsync()
        {
            _interopProcess.StandardInput.WriteLine($"update {ProjectService.GetActiveProjectDirectory()}");

            // Wait for the JSON response asynchronously
            string jsonResponse = await _interopProcess.StandardOutput.ReadLineAsync();

            // Parse and handle the JSON response
            if (string.IsNullOrWhiteSpace(jsonResponse))
                throw new InvalidOperationException("No response received from Update command.");

            try
            {
                var root = Newtonsoft.Json.Linq.JObject.Parse(jsonResponse);
                var response = root["response"];
                if (response == null)
                    throw new InvalidOperationException("Invalid Update response format: missing 'response' property.");

                var status = response["status"]?.Value<string>();
                var message = response["message"]?.Value<string>();

                if (status == "error")
                    throw new InvalidOperationException($"Update command returned error: {message}");

                if (status != "success")
                    throw new InvalidOperationException($"Unexpected status in Update response: {status}");

                // Success: nothing to return, but you could log message if needed
            }
            catch (Newtonsoft.Json.JsonException ex)
            {
                throw new InvalidOperationException("Failed to parse Update JSON response.", ex);
            }
        }

        public string GetText(BitmapImage bitmapImage)
        {
            if (bitmapImage == null)
            {
                throw new ArgumentNullException(nameof(bitmapImage), "BitmapImage cannot be null.");
            }

            // Save the BitmapImage to a temporary file
            string tempFilePath = SaveBitmapImageToTemp(bitmapImage);

            // Send the "ocr" command to the running process
            _interopProcess.StandardInput.WriteLine($"ocr {tempFilePath}");

            // Read the JSON response from the process
            string jsonResponse = _interopProcess.StandardOutput.ReadLine();

            try
            {
                File.Delete(tempFilePath);
            }
            catch (IOException ex)
            {
                Debug.WriteLine($"Failed to delete temporary file: {ex.Message}");
                throw;
            }

            // Parse the JSON and extract the textFound field
            if (string.IsNullOrWhiteSpace(jsonResponse))
                throw new InvalidOperationException("No response received from OCR process.");

            try
            {
                var root = JObject.Parse(jsonResponse);
                var response = root["response"];
                if (response == null)
                    throw new InvalidOperationException("Invalid OCR response format: missing 'response' property.");

                var status = response["status"];
                if (status == null)
                    throw new InvalidOperationException("Invalid OCR response format: missing 'status' property.");
                var message = response["message"];
                if (status == null)
                    throw new InvalidOperationException("Invalid OCR response format: missing 'status' property.");

                if (status.Value<string>() == "error")
                    throw new InvalidOperationException($"OCR process returned error: {message}");

                var data = response["data"];
                if (data == null)
                    throw new InvalidOperationException("Invalid OCR response format: missing 'data' property.");
                var textFound = data["textFound"];
                if (textFound == null)
                    throw new InvalidOperationException("Invalid OCR response format: missing 'textFound' property.");

                return textFound.Value<string>() ?? string.Empty;
            }
            catch (Newtonsoft.Json.JsonException ex)
            {
                throw new InvalidOperationException("Failed to parse OCR JSON response.", ex);
            }
        }

        private string SaveBitmapImageToTemp(BitmapImage bitmapImage)
        {
            string tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");

            // Ensure the directory exists
            string directoryPath = Path.GetDirectoryName(tempFilePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(fileStream);
            }

            // Verify the file exists
            if (!File.Exists(tempFilePath))
            {
                throw new IOException($"Failed to create the temporary file at {tempFilePath}");
            }

            return tempFilePath;
        }
    }
}
