using System.Runtime.InteropServices;

namespace POC_Tesseract.UserInterface
{
    /// <summary>
    ///  All the virtual key codes can be found on the microsoft website 
    ///  https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
    /// </summary>
    public class Keyboard
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, nuint dwExtraInfo);

        private const uint KEYEVENTF_KEYDOWN = 0x0000; // Key down flag
        private const uint KEYEVENTF_KEYUP = 0x0002;   // Key up flag

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern short VkKeyScan(char ch);

        /// <summary>
        /// Simulates typing a string by sending key down and key up events for each character.
        /// </summary>
        /// <param name="text">The string to type.</param>
        public static void WriteString(string text)
        {
            foreach (char c in text)
            {
                // Get the virtual key code and shift state for the character
                short keyInfo = VkKeyScan(c);
                if (keyInfo == -1)
                {
                    // Character cannot be mapped to a virtual key
                    continue;
                }

                byte virtualKey = (byte)(keyInfo & 0xFF);
                bool shiftRequired = (keyInfo & 0x0100) != 0;

                // Press Shift if required
                if (shiftRequired)
                {
                    KeyDown(0x10); // Virtual key code for Shift
                }

                // Simulate key press
                KeyDown(virtualKey);
                Thread.Sleep(50); // Small delay to simulate natural typing
                KeyUp(virtualKey);

                // Release Shift if it was pressed
                if (shiftRequired)
                {
                    KeyUp(0x10); // Virtual key code for Shift
                }
            }
        }

        /// <summary>
        /// Simulates a key press (key down).
        /// </summary>
        /// <param name="keyCode">The virtual key code of the key to press.</param>
        public static void KeyDown(byte keyCode)
        {
            keybd_event(keyCode, 0, KEYEVENTF_KEYDOWN, nuint.Zero);
        }

        /// <summary>
        /// Simulates a key release (key up).
        /// </summary>
        /// <param name="keyCode">The virtual key code of the key to release.</param>
        public static void KeyUp(byte keyCode)
        {
            keybd_event(keyCode, 0, KEYEVENTF_KEYUP, nuint.Zero);
        }
    }
}
