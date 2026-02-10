using SharpHook;

namespace VisionTest.Core.Input;

/// <summary>
/// This class provides methods to simulate keyboard input.
/// </summary>
public class Keyboard :IKeyboard
{
    private EventSimulator simulator = new();

    public void PressKey(KeyCode key)
    {
        simulator.SimulateKeyPress((SharpHook.Data.KeyCode)key);
        simulator.SimulateKeyRelease((SharpHook.Data.KeyCode)key);
    }

    public void KeyDown(KeyCode key)
    {
        simulator.SimulateKeyPress((SharpHook.Data.KeyCode)key);
    }

    public void KeyUp(KeyCode key)
    {
        simulator.SimulateKeyRelease((SharpHook.Data.KeyCode)key);
    }

    public void TypeText(string text)
    {
        simulator.SimulateTextEntry(text);
    }

    public void SendModifiedKeyStroke(IEnumerable<KeyCode> modifiers, KeyCode key)
    {
        foreach (var modifier in modifiers)
        {
            KeyDown(modifier);
        }

        PressKey(key);

        foreach (var modifier in modifiers.Reverse())
        {
            KeyUp(modifier);
        }

    }

    public void ReleaseAllKeys()
    {
        foreach (var key in Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>())
        {
            KeyUp(key);
        }
    }
}
