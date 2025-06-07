using System.Windows.Forms;
using System.Drawing;
using VisionTest.Core.Services;
using VisionTest.Core.Input;
using VisionTest.Core.Utils;
using VisionTest.Core.Models;
using NUnit.Framework.Internal.Execution;

namespace VisionTest.Tests.TestExecutorTests
{
    internal class WaitForTextTests
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
                    form.Activate();
                    form.BringToFront();
                    form.Focus();
                    form.WindowState = FormWindowState.Maximized; // Maximize the form to ensure visibility
                    await Task.Delay(1000); // Let form render and become visible

                    try
                    {
                        // Call the method under test
                        var actualPoint = appli.WaitFor(targetText); // screen-relative center point

                        // Calculate the expected screen-relative center of the label
                        var screenLocation = form.PointToScreen(targetLabel.Location);
                        var labelSize = targetLabel.PreferredSize;
                        var expectedCenter = new Point(
                            (int)((screenLocation.X + labelSize.Width / 2)*Core.Input.Screen.ScaleFactor),
                            (int)((screenLocation.Y + labelSize.Height / 2) * Core.Input.Screen.ScaleFactor)
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
                    form.WindowState = FormWindowState.Maximized; // Maximize the form to ensure visibility
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
        public async Task Waitfor_Text_or_imagePath_ShouldReturnCenterOfTextOnScreen()
        {

            await Waitfor_Method_Path_ShouldReturnCenterOfTextOnScreen(s => appli.WaitFor(s, "C:\\Users\\guill\\Programmation\\dotNET_doc\\VisionTest\\VisionTest.Tests\\images\\cottonLike.png"));
        }

        [Test]
        public void TryWaitfor_ScreenElement_True()
        {
            var result = appli.TryWaitFor(new ScreenElement() { Texts = { "File", Guid.NewGuid().ToString() }, Images = {new Bitmap("C:\\Users\\guill\\Programmation\\dotNET_doc\\VisionTest\\VisionTest.Tests\\images\\cottonLike2.png") } }, out _);
            Assert.That(result, Is.True, "Expected TryWaitFor to return true for existing text.");
        }

        [Test]
        public void TryWaitfor_ScreenElement_False()
        {
            Rectangle? rect;
            var result = appli.TryWaitFor(new ScreenElement() { Texts = { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() } }, out rect);
            Assert.That(result, Is.False, "Expected TryWaitFor to return False for existing text.");
            Assert.That(rect, Is.Null, "Expected rectangle to be null when text is not found.");
        }

        [Test]
        public void TryWaitfor_Text_True()
        {
            var result = appli.TryWaitFor("File", out _);
            Assert.That(result, Is.True, "Expected TryWaitFor to return true for existing text.");
        }

        [Test]
        public void TryWaitfor_Text_False()
        {
            Rectangle? rect;
            var result = appli.TryWaitFor(Guid.NewGuid().ToString(), out rect);
            Assert.That(result, Is.False, "Expected TryWaitFor to return False for existing text.");
            Assert.That(rect, Is.Null, "Expected rectangle to be null when text is not found.");
        }


        private async Task Waitfor_Method_Path_ShouldReturnCenterOfTextOnScreen(Func<string, Rectangle> methodUnderTest)
        {
            var targetText = "TargetLabelText";
            var tcs = new TaskCompletionSource<(Point? actual, Point? expected, Exception? exception)>();

            var uiThread = new Thread(() =>
            {
                using var form = new Form
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
                    form.Activate();
                    form.Focus();
                    form.BringToFront();

                    form.WindowState = FormWindowState.Maximized; // Maximize the form to ensure visibility
                    await Task.Delay(1000); // Let form render and become visible

                    try
                    {
                        // Call the method under test
                        var actualPoint = methodUnderTest(targetText); // screen-relative center point

                        // Calculate the expected screen-relative center of the label
                        var screenLocation = form.PointToScreen(targetLabel.Location);
                        var labelSize = targetLabel.PreferredSize;
                        var expectedCenter = new Point(
                            (int)((screenLocation.X + labelSize.Width / 2) * int.Parse(TestResources.ScreenScale.TrimEnd('%')) / 100.0f),
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
                        form.Dispose();
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
    }
}

