using System.Configuration;
using System.Xml.Linq;

namespace TestsImplementation
{

    [SetUpFixture]
    public class TestSetup
    {
        [OneTimeSetUp]
        public void GlobalSetup()
        {
            // Charger le fichier XML
            var configFilePath = @"..\..\..\..\POC Tesseract\App.config"; // Chemin relatif vers le fichier XML
            var configXml = XDocument.Load(configFilePath);

            // Parcourir toutes les clés de la section appSettings
            var appSettings = configXml
                .Descendants("appSettings")
                .Descendants("add");

            foreach (var setting in appSettings)
            {
                var key = setting.Attribute("key")?.Value;
                var value = setting.Attribute("value")?.Value;

                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    AddUpdateAppSettings(key, value);
                }
            }
        }

        private static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.Error.WriteLine("Error writing app settings");
                throw;
            }
        }
    }
}
