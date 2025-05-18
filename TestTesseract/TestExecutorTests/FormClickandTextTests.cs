using System.Windows.Forms;
using System.Drawing;
using Core.Services;
using Core.Input;
using Core.Utils;
using Core.Models;

namespace Tests.TestExecutorTests
{
    internal class FormClickandTextTests
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
                    new Keyboard().TypeText(expectedText); // <--- Call your method here

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

        [Test]
        public async Task Waitfor_ShouldReturnCenterOfTextOnScreen()
        {
            var targetText = "TargetLabelText";
            var tcs = new TaskCompletionSource<(Point? actual, Point? expected, Exception? exception)>();

            var uiThread = new Thread(() =>
            {
                var form = new Form
                {
                    Width = 800,
                    Height = 600,
                    BackColor = Color.White
                };

                // Add noise: random labels with different fonts and styles
                for (int i = 0; i < 30; i++)
                {
                    var label = new Label
                    {
                        Text = $"RandomLabel {i}",
                        Font = new Font("Arial", 8 + i % 5, i % 2 == 0 ? FontStyle.Bold : FontStyle.Italic),
                        Location = new Point(10, i * 20 + 10),
                        AutoSize = true
                    };
                    form.Controls.Add(label);
                }

                // Add the actual label to be found
                var targetLabel = new Label
                {
                    Text = targetText,
                    Font = new Font("Segoe UI", 16, FontStyle.Bold),
                    Location = new Point(300, 400),
                    AutoSize = true
                };
                form.Controls.Add(targetLabel);

                form.Load += async (sender, args) =>
                {
                    form.BringToFront();
                    form.Activate();
                    await Task.Delay(1000); // Let form render and become visible

                    try
                    {
                        // Call the method under test
                        var actualPoint = appli.WaitFor(targetText); // screen-relative center point

                        // Calculate the expected screen-relative center of the label
                        var screenLocation = form.PointToScreen(targetLabel.Location);
                        var labelSize = targetLabel.PreferredSize;
                        var expectedCenter = new Point(
                            (int)((screenLocation.X + labelSize.Width / 2)*new Core.Input.Screen().ScaleFactor),
                            (int)((screenLocation.Y + labelSize.Height / 2) * new Core.Input.Screen().ScaleFactor)
                        );

                        tcs.SetResult((actualPoint.Center(), expectedCenter, null));
                    }
                    catch (TimeoutException ex)
                    {
                        // Capture the exception and set it in the TaskCompletionSource
                        tcs.SetResult((null, null, ex));
                    }
                    finally
                    {
                        form.Close();
                    }
                };

                Application.Run(form);
            });

            uiThread.SetApartmentState(ApartmentState.STA);
            uiThread.Start();

            var (actual, expected, exception) = await tcs.Task;

            if (exception != null)
            {
                Assert.Fail($"An exception was thrown: {exception.Message}");
            }
            else
            {
                var tolerance = 20;

                Assert.That(actual!.Value.X, Is.InRange(expected!.Value.X - tolerance, expected.Value.X + tolerance), "X coordinate mismatch");
                Assert.That(actual.Value.Y, Is.InRange(expected.Value.Y - tolerance, expected.Value.Y + tolerance), "Y coordinate mismatch");
            }
        }

        [Test]
        public async Task WaitforElement_ShouldReturnCenterOfTextOnScreen()
        {
            var targetText = "TargetLabelText";
            var targetElement = new ScreenElement();
            targetElement.Texts.Add(targetText);
            var tcs = new TaskCompletionSource<(Point? actual, Point? expected, Exception? exception)>();

            var uiThread = new Thread(() =>
            {
                var form = new Form
                {
                    Width = 800,
                    Height = 600,
                    BackColor = Color.White
                };

                // Add noise: random labels with different fonts and styles
                for (int i = 0; i < 30; i++)
                {
                    var label = new Label
                    {
                        Text = $"RandomLabel {i}",
                        Font = new Font("Arial", 8 + i % 5, i % 2 == 0 ? FontStyle.Bold : FontStyle.Italic),
                        Location = new Point(10, i * 20 + 10),
                        AutoSize = true
                    };
                    form.Controls.Add(label);
                }

                // Add the actual label to be found
                var targetLabel = new Label
                {
                    Text = targetText,
                    Font = new Font("Segoe UI", 16, FontStyle.Bold),
                    Location = new Point(300, 400),
                    AutoSize = true
                };
                form.Controls.Add(targetLabel);

                form.Load += async (sender, args) =>
                {
                    form.BringToFront();
                    form.Activate();
                    await Task.Delay(1000); // Let form render and become visible

                    try
                    {
                        // Call the method under test
                        var actualPoint = appli.WaitFor(targetElement); // screen-relative center point

                        // Calculate the expected screen-relative center of the label
                        var screenLocation = form.PointToScreen(targetLabel.Location);
                        var labelSize = targetLabel.PreferredSize;
                        var expectedCenter = new Point(
                            (int)((screenLocation.X + labelSize.Width / 2)* int.Parse(TestResources.ScreenScale.TrimEnd('%'))/100.0f),
                            (int)((screenLocation.Y + labelSize.Height / 2) * int.Parse(TestResources.ScreenScale.TrimEnd('%')) / 100.0f)
                        );

                        tcs.SetResult((actualPoint.Center(), expectedCenter, null));
                    }
                    catch (TimeoutException ex)
                    {
                        // Capture the exception and set it in the TaskCompletionSource
                        tcs.SetResult((null, null, ex));
                    }
                    finally
                    {
                        form.Close();
                    }
                };

                Application.Run(form);
            });

            uiThread.SetApartmentState(ApartmentState.STA);
            uiThread.Start();

            var (actual, expected, exception) = await tcs.Task;

            if (exception != null)
            {
                Assert.Fail($"An exception was thrown: {exception.Message}");
            }
            else
            {
                var tolerance = 20;

                Assert.That(actual!.Value.X, Is.InRange(expected!.Value.X - tolerance, expected.Value.X + tolerance), "X coordinate mismatch");
                Assert.That(actual.Value.Y, Is.InRange(expected.Value.Y - tolerance, expected.Value.Y + tolerance), "Y coordinate mismatch");
            }
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

