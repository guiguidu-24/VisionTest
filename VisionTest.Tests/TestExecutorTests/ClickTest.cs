using System.Windows.Forms;
using System.Drawing;
using VisionTest.Core.Services;
using VisionTest.Core.Input;
using VisionTest.Core.Utils;
using VisionTest.Core.Models;

namespace VisionTest.Tests.TestExecutorTests
{
    internal class ClickTest
    {
        private TestExecutor appli;

        [SetUp]
        public void Setup()
        {
            // Initialisation avant chaque test
            appli = new TestExecutor();
        }

        [TearDown]
        public void TearDown()
        {
            appli = null!;
        }

        [Test]
        public async Task Click_ShouldClickButtonOnScreen()
        {
            var tcs = new TaskCompletionSource<bool>();

            var uiThread = new Thread(() =>
            {
                var form = new Form
                {
                    StartPosition = FormStartPosition.Manual,
                    Location = new Point(100, 100), // Ensure predictable location
                    Size = new Size(200, 200)
                };

                var button = new Button
                {
                    Text = "Click Me",
                    Size = new Size(2, 2), // Small button for precision
                    Location = new Point(50, 50) // Position inside the form
                };

                bool buttonClicked = false;

                button.Click += (sender, args) => buttonClicked = true;
                form.Controls.Add(button);

                form.Load += async (sender, args) =>
                {
                    form.BringToFront();
                    form.Activate();
                    await Task.Delay(500); // Let form render and become visible

                    // Calculate the button's screen coordinates
                    var buttonScreenLocation = form.PointToScreen(button.Location);
                    var clickPoint = new Point((int) ((buttonScreenLocation.X + button.Width / 2) * int.Parse(TestResources.ScreenScale.TrimEnd('%'))/100f), (int)((buttonScreenLocation.Y + button.Height / 2) * int.Parse(TestResources.ScreenScale.TrimEnd('%')) / 100f));

                    // Act
                    appli.Click(clickPoint);
                    await Task.Delay(100); // Allow time for the click to be processed

                    tcs.SetResult(buttonClicked);

                    form.Close();
                };

                Application.Run(form);
            });

            uiThread.SetApartmentState(ApartmentState.STA);
            uiThread.Start();

            bool buttonClicked = await tcs.Task;

            Assert.That(buttonClicked, Is.True, "The button should have been clicked.");
        }
    
    }
}

