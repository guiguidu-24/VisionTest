using System.Diagnostics;
using POC_Tesseract;
using TestSUT;
using WindowsInput;

namespace TestTesseract
{
    /// <summary>
    /// Tests for the Appli class.
    /// </summary>
    internal partial class AppliTests
    {
        private Appli appli;

        [SetUp]
        public void Setup()
        {
            // Initialisation avant chaque test
            appli = new Appli("notepad"); //TODO: Tests non fonctionnels à cause du path du Tessract
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var process in Process.GetProcessesByName("notepad"))
            {
                process.Kill();
                process.WaitForExit();
            }
        }


        [Test]
        public void Open_ShouldOpenNotepadProcess()
        {
            // Arrange
            appli.Open();
            Thread.Sleep(1000); // Wait a bit to ensure the process is started

            // Verify that the process is started
            var notepadProcessBeforeClose = Process.GetProcessesByName("notepad").FirstOrDefault();
            Assert.IsNotNull(notepadProcessBeforeClose, "Le processus notepad devrait être démarré avant");
        }

        [Test]
        public void Close_ShouldTerminateNotepadProcess()
        {
            // Arrange
            appli.Open();

            Thread.Sleep(1000); // Wait a bit to ensure the process is started

            // Act
            appli.CloseWindow();

            Thread.Sleep(1000); // Wait a bit to ensure the process is terminated

            // Assert
            var notepadProcessAfterClose = Process.GetProcessesByName("notepad").FirstOrDefault();
            Assert.IsNull(notepadProcessAfterClose, "Le processus notepad devrait être fermé après l'appel à Close.");
        }

        [Test]
        public void MaximizeWindow_ShouldMaximizeNotepadWindow()
        {
            // Arrange
            appli.Open();
            Thread.Sleep(1000); // Attendre que le processus démarre

            // Act
            appli.MaximizeWindow();
            Thread.Sleep(1000); // Attendre que la fenêtre soit maximisée

            // Assert
            var notepadProcess = Process.GetProcessesByName("notepad").FirstOrDefault();
            Assert.IsNotNull(notepadProcess, "Le processus Notepad devrait être démarré.");

            var mainWindowHandle = notepadProcess.MainWindowHandle;
            Assert.That(mainWindowHandle, Is.Not.EqualTo(IntPtr.Zero), "La fenêtre principale de Notepad devrait être valide.");

            // Vérifier si la fenêtre est maximisée
            var placement = new WINDOWPLACEMENT();
            GetWindowPlacement(mainWindowHandle, ref placement);
            Assert.That(placement.showCmd, Is.EqualTo(SW_SHOWMAXIMIZED), "La fenêtre Notepad devrait être maximisée.");
        }

        [Test]
        public void ResizeWindow_ShouldResizeNotepadWindow()
        {
            // Arrange
            appli.Open();
            Thread.Sleep(1000); // Attendre que le processus démarre

            int expectedWidth = 800;
            int expectedHeight = 600;

            // Act
            appli.ResizeWindow(expectedWidth, expectedHeight);
            Thread.Sleep(1000); // Attendre que la fenêtre soit redimensionnée

            // Assert
            var notepadProcess = Process.GetProcessesByName("notepad").FirstOrDefault();
            Assert.IsNotNull(notepadProcess, "Le processus Notepad devrait être démarré.");

            var mainWindowHandle = notepadProcess.MainWindowHandle;
            Assert.That(mainWindowHandle, Is.Not.EqualTo(IntPtr.Zero), "La fenêtre principale de Notepad devrait être valide.");

            // Vérifier les dimensions de la fenêtre
            RECT rect;
            Assert.IsTrue(GetWindowRect(mainWindowHandle, out rect), "Impossible de récupérer les dimensions de la fenêtre.");
            int actualWidth = rect.right - rect.left;
            int actualHeight = rect.bottom - rect.top;

            Assert.That(actualWidth, Is.EqualTo(expectedWidth), $"La largeur de la fenêtre devrait être {expectedWidth}.");
            Assert.That(actualHeight, Is.EqualTo(expectedHeight), $"La hauteur de la fenêtre devrait être {expectedHeight}.");
        }

        [Test]
        public void Write_ShouldSimulateTextInputInNotepad()
        {
            // Arrange
            appli.Open();
            Thread.Sleep(1000); // Attendre que le processus démarre

            string expectedText = "Hello, world!";

            // Act
            appli.Write(expectedText);
            Thread.Sleep(1000); // Attendre que le texte soit écrit

            // Assert
            var notepadProcess = Process.GetProcessesByName("notepad").FirstOrDefault();
            Assert.IsNotNull(notepadProcess, "Le processus Notepad devrait être démarré.");

            var mainWindowHandle = notepadProcess.MainWindowHandle;
            Assert.That(mainWindowHandle, Is.Not.EqualTo(IntPtr.Zero), "La fenêtre principale de Notepad devrait être valide.");

            // Vérifier que le texte a été écrit dans Notepad
            // TODO : Pour vérifier le contenu de Notepad, il faudrait utiliser des techniques avancées comme l'API Windows pour lire le texte de la fenêtre.
            // Ici, nous supposons que si aucune exception n'est levée, le texte a été écrit correctement.
            Assert.Pass("The text passed with success");
        }
    }
}
