using System.Windows.Forms;
using System.Drawing;
using VisionTest.Core.Input;
using VisionTest.Core.Utils;
using VisionTest.Core.Models;

namespace VisionTest.Tests.Core.Models.LocatorVTests
{
    internal class WaitForTextTests
    {
        [SetUp]
        public void Setup()
        {
            // No setup needed for LocatorV tests
        }

        [TearDown]
        public void TearDown()
        {
            // No cleanup needed
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
                        // Call the method under test - Use LocatorV to wait for text
                        var locator = new LocatorV(targetText);
                        var actualRect = await locator.WaitForAsync();
                        var actualPoint = actualRect.Center();

                        // Calculate the expected screen-relative center of the label
                        var screenLocation = form.PointToScreen(targetLabel.Location);
                        var labelSize = targetLabel.PreferredSize;
                        var expectedCenter = new Point(
                            (int)((screenLocation.X + labelSize.Width / 2)*new VisionTest.Core.Input.Screen().ScaleFactor),
                            (int)((screenLocation.Y + labelSize.Height / 2) * new VisionTest.Core.Input.Screen().ScaleFactor)
                        );

                        tcs.SetResult((actualPoint, expectedCenter, null));
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
                        // Call the method under test - Use LocatorV to wait for text
                        var locator = new LocatorV(targetText);
                        var actualRect = await locator.WaitForAsync();
                        var actualPoint = actualRect.Center();

                        // Calculate the expected screen-relative center of the label
                        var screenLocation = form.PointToScreen(targetLabel.Location);
                        var labelSize = targetLabel.PreferredSize;
                        var expectedCenter = new Point(
                            (int)((screenLocation.X + labelSize.Width / 2)* int.Parse(TestResources.ScreenScale.TrimEnd('%'))/100.0f),
                            (int)((screenLocation.Y + labelSize.Height / 2) * int.Parse(TestResources.ScreenScale.TrimEnd('%')) / 100.0f)
                        );

                        tcs.SetResult((actualPoint, expectedCenter, null));
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
            // This test now uses LocatorV with multiple SimpleLocatorV objects to simulate text OR image search
            var imagePath = "C:\\Users\\guill\\Programmation\\dotNET_doc\\VisionTest\\VisionTest.Tests\\images\\cottonLike.png";
            var image = new Bitmap(imagePath);
            await Waitfor_Method_Path_ShouldReturnCenterOfTextOnScreen(async s => 
            {
                // Create LocatorV with both text and image options
                var textLocator = new SimpleLocatorV(text: s);
                var imageLocator = new SimpleLocatorV(image: image);
                var locator = new LocatorV(new[] { textLocator, imageLocator });
                return await locator.WaitForAsync();
            });
        }

        [Test]
        public async Task TryWaitfor_MultipleTexts_True()
        {
            // Test LocatorV with multiple text options - should find "File"
            var textLocator1 = new SimpleLocatorV(text: "File");
            var textLocator2 = new SimpleLocatorV(text: Guid.NewGuid().ToString());
            var imagePath = "C:\\Users\\guill\\Programmation\\dotNET_doc\\VisionTest\\VisionTest.Tests\\images\\cottonLike2.png";
            var imageLocator = new SimpleLocatorV(image: new Bitmap(imagePath));
            var locator = new LocatorV(new[] { textLocator1, textLocator2, imageLocator });
            
            var (success, area) = await locator.TryWaitForAsync();
            Assert.That(success, Is.True, "Expected TryWaitFor to return true for existing text.");
            Assert.That(area, Is.Not.Null, "Expected area to be not null when text is found.");
        }

        [Test]
        public async Task TryWaitfor_MultipleTexts_False()
        {
            // Test LocatorV with multiple random text options - should not find any
            var textLocator1 = new SimpleLocatorV(text: Guid.NewGuid().ToString());
            var textLocator2 = new SimpleLocatorV(text: Guid.NewGuid().ToString());
            var locator = new LocatorV(new[] { textLocator1, textLocator2 });
            
            var (success, area) = await locator.TryWaitForAsync();
            Assert.That(success, Is.False, "Expected TryWaitFor to return False for non-existing text.");
            Assert.That(area, Is.Null, "Expected area to be null when text is not found.");
        }

        [Test]
        public async Task TryWaitfor_Text_True()
        {
            var locator = new LocatorV("File");
            var (success, area) = await locator.TryWaitForAsync();
            Assert.That(success, Is.True, "Expected TryWaitFor to return true for existing text.");
            Assert.That(area, Is.Not.Null, "Expected area to be not null when text is found.");
        }

        [Test]
        public async Task TryWaitfor_Text_False()
        {
            var locator = new LocatorV(Guid.NewGuid().ToString());
            var (success, area) = await locator.TryWaitForAsync();
            Assert.That(success, Is.False, "Expected TryWaitFor to return False for non-existing text.");
            Assert.That(area, Is.Null, "Expected area to be null when text is not found.");
        }


        private async Task Waitfor_Method_Path_ShouldReturnCenterOfTextOnScreen(Func<string, Task<Rectangle>> methodUnderTest)
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
                        var actualRect = await methodUnderTest(targetText);
                        var actualPoint = actualRect.Center();

                        // Calculate the expected screen-relative center of the label
                        var screenLocation = form.PointToScreen(targetLabel.Location);
                        var labelSize = targetLabel.PreferredSize;
                        var expectedCenter = new Point(
                            (int)((screenLocation.X + labelSize.Width / 2) * int.Parse(TestResources.ScreenScale.TrimEnd('%')) / 100.0f),
                            (int)((screenLocation.Y + labelSize.Height / 2) * int.Parse(TestResources.ScreenScale.TrimEnd('%')) / 100.0f)
                        );

                        tcs.SetResult((actualPoint, expectedCenter, null));
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

