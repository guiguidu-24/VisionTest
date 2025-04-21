using POC_Tesseract;
using WindowsInput;
using System.Windows.Forms;
using System.Net;

namespace TestTesseract
{
    internal class AppliTestsForm
    {
        private Appli appli;

        [SetUp]
        public void Setup()
        {
            // Initialisation avant chaque test
            appli = new Appli("");
        }

        [TearDown]
        public void TearDown()
        {
            appli = null!;
        }

        [Test]
        public async Task Write_ShouldTypeCorrectCharacters()
        {
            var expectedText = "HelloWorld";
            var tcs = new TaskCompletionSource<string>();

            var uiThread = new Thread(() =>
            {
                var form = new Form();
                var textBox = new TextBox { Dock = DockStyle.Fill };
                form.Controls.Add(textBox);

                form.Load += async (sender, args) =>
                {
                    // Give the form time to focus the textbox
                    await Task.Delay(300);
                    textBox.Focus();
                    await Task.Delay(300); // Wait for focus

                    // Simulate key presses
                    appli.Write(expectedText); // <--- Call your method here

                    await Task.Delay(300); // Wait for keys to be processed
                    tcs.SetResult(textBox.Text);
                    form.Close();
                };

                Application.Run(form);
            });

            uiThread.SetApartmentState(ApartmentState.STA); // Windows Forms needs STA
            uiThread.Start();

            var actualText = await tcs.Task;
            Assert.That(actualText, Is.EqualTo(expectedText));
        }
    }
}
