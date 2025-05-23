using WindowsInput.Events;

namespace VisionTest.Core.Input
{
    public interface IKeyboard
    {
        /// <summary>
        /// Sends a single key press (press + release).
        /// </summary>
        void PressKey(KeyCode key);

        /// <summary>
        /// Presses down a key (without releasing it).
        /// </summary>
        void KeyDown(KeyCode key);

        /// <summary>
        /// Releases a key that was previously pressed.
        /// </summary>
        void KeyUp(KeyCode key);

        /// <summary>
        /// Types a full string (e.g., "Hello world").
        /// </summary>
        void TypeText(string text);

        /// <summary>
        /// Sends a key combination, e.g., Ctrl + C.
        /// </summary>
        void SendModifiedKeyStroke(IEnumerable<KeyCode> modifiers, KeyCode key);

        /// <summary>
        /// Clears all held keys (safety mechanism).
        /// </summary>
        void ReleaseAllKeys();
    }
}
