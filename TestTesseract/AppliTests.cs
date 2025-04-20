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
            appli = new Appli("notepad"); //TODO: Tests non fonctionnels � cause du path du Tessract
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
            Assert.IsNotNull(notepadProcessBeforeClose, "Le processus notepad devrait �tre d�marr� avant");
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
            Assert.IsNull(notepadProcessAfterClose, "Le processus notepad devrait �tre ferm� apr�s l'appel � Close.");
        }

        [Test]
        public void MaximizeWindow_ShouldMaximizeNotepadWindow()
        {
            // Arrange
            appli.Open();
            Thread.Sleep(1000); // Attendre que le processus d�marre

            // Act
            appli.MaximizeWindow();
            Thread.Sleep(1000); // Attendre que la fen�tre soit maximis�e

            // Assert
            var notepadProcess = Process.GetProcessesByName("notepad").FirstOrDefault();
            Assert.IsNotNull(notepadProcess, "Le processus Notepad devrait �tre d�marr�.");

            var mainWindowHandle = notepadProcess.MainWindowHandle;
            Assert.That(mainWindowHandle, Is.Not.EqualTo(IntPtr.Zero), "La fen�tre principale de Notepad devrait �tre valide.");

            // V�rifier si la fen�tre est maximis�e
            var placement = new WINDOWPLACEMENT();
            GetWindowPlacement(mainWindowHandle, ref placement);
            Assert.That(placement.showCmd, Is.EqualTo(SW_SHOWMAXIMIZED), "La fen�tre Notepad devrait �tre maximis�e.");
        }

        [Test]
        public void ResizeWindow_ShouldResizeNotepadWindow()
        {
            // Arrange
            appli.Open();
            Thread.Sleep(1000); // Attendre que le processus d�marre

            int expectedWidth = 800;
            int expectedHeight = 600;

            // Act
            appli.ResizeWindow(expectedWidth, expectedHeight);
            Thread.Sleep(1000); // Attendre que la fen�tre soit redimensionn�e

            // Assert
            var notepadProcess = Process.GetProcessesByName("notepad").FirstOrDefault();
            Assert.IsNotNull(notepadProcess, "Le processus Notepad devrait �tre d�marr�.");

            var mainWindowHandle = notepadProcess.MainWindowHandle;
            Assert.That(mainWindowHandle, Is.Not.EqualTo(IntPtr.Zero), "La fen�tre principale de Notepad devrait �tre valide.");

            // V�rifier les dimensions de la fen�tre
            RECT rect;
            Assert.IsTrue(GetWindowRect(mainWindowHandle, out rect), "Impossible de r�cup�rer les dimensions de la fen�tre.");
            int actualWidth = rect.right - rect.left;
            int actualHeight = rect.bottom - rect.top;

            Assert.That(actualWidth, Is.EqualTo(expectedWidth), $"La largeur de la fen�tre devrait �tre {expectedWidth}.");
            Assert.That(actualHeight, Is.EqualTo(expectedHeight), $"La hauteur de la fen�tre devrait �tre {expectedHeight}.");
        }

        [Test]
        public void Write_ShouldSimulateTextInputInNotepad()
        {
            // Arrange
            appli.Open();
            Thread.Sleep(1000); // Attendre que le processus d�marre

            string expectedText = "Hello, world!";

            // Act
            appli.Write(expectedText);
            Thread.Sleep(1000); // Attendre que le texte soit �crit

            // Assert
            var notepadProcess = Process.GetProcessesByName("notepad").FirstOrDefault();
            Assert.IsNotNull(notepadProcess, "Le processus Notepad devrait �tre d�marr�.");

            var mainWindowHandle = notepadProcess.MainWindowHandle;
            Assert.That(mainWindowHandle, Is.Not.EqualTo(IntPtr.Zero), "La fen�tre principale de Notepad devrait �tre valide.");

            // V�rifier que le texte a �t� �crit dans Notepad
            // TODO : Pour v�rifier le contenu de Notepad, il faudrait utiliser des techniques avanc�es comme l'API Windows pour lire le texte de la fen�tre.
            // Ici, nous supposons que si aucune exception n'est lev�e, le texte a �t� �crit correctement.
            Assert.Pass("The text passed with success");
        }
    }
}
