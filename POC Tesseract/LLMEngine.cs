

using System.Drawing.Imaging;
using System.Net.Http.Json;

namespace POC_Tesseract
{
    public class LLMEngine
    {
        private readonly HttpClient _client = new();
        private const string OllamaUrl = "http://localhost:11434/api/generate";
        private string llmModel = "llava";

        public LLMEngine(string model)
        {
            llmModel = model;
        }


        public bool Find(Bitmap image, string text, out Rectangle area) //TODO on pourrait rendre ces méthodes asynchrones si la réponse prend trop de temps (à vérifier)
        {
            area = Rectangle.Empty;

            string base64Image = BitmapToBase64(image);
            var request = new
            {
                model = "llava",
                prompt = $"Analyze the provided image and determine if the following text is present: \"{text}\". " +
                         "If the text is found, return the result as 'true' along with the coordinates of a rectangle " +
                         "that encloses the text in the format: { \"found\": true, \"rectangle\": { \"x\": <x>, \"y\": <y>, \"width\": <width>, \"height\": <height> } }. " +
                         "If the text is not found, return the result as { \"found\": false }.",
                images = new[] { base64Image }
            };

            var response = _client.PostAsJsonAsync(OllamaUrl, request).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
                return false;

            string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            //TODO : Si la réponse est oui ou non, plutôt que ça
            //Condition sur le contenu de la réponse
            if (content.Contains(text, StringComparison.OrdinalIgnoreCase)) //TODO : implémenter les coordonnées du rectangle (fictif pour l'instant)
            {
                // Rectangle fictif (dans une vraie app, il faudrait utiliser OCR ou vision model pour extraire les coords)
                area = new Rectangle(50, 50, 100, 30);
                return true;
            }

            return false;
        }

        public bool Find(Bitmap image, Bitmap target, out Rectangle area)
        {
            area = Rectangle.Empty;

            // Idéalement ici, on compare visuellement target dans image avec template matching
            // En attendant, on utilise LLaVA comme "descripteur"
            string base64Image = BitmapToBase64(image);
            string base64Target = BitmapToBase64(target);

            var request = new
            {
                model = llmModel,
                prompt = $"Est-ce que cette petite image est présente dans l'image principale ?",
                images = new[] { base64Image, base64Target}
            };

            var response = _client.PostAsJsonAsync(OllamaUrl, request).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
                return false;

            string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            if (content.Contains("oui", StringComparison.OrdinalIgnoreCase) || content.Contains("present", StringComparison.OrdinalIgnoreCase))
            {
                area = new Rectangle(100, 100, target.Width, target.Height); //TODO : implémenter la zone de l'image
                return true;
            }

            return false;
        }

        private string BitmapToBase64(Bitmap bmp) //TODO: Faire directement la conversion bitmap -> base64
        {
            using var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            return Convert.ToBase64String(ms.ToArray());
        }
    }

}
