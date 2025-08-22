using WindowsInput;
using WindowsInput.Events;

namespace VisionTest.Core.Input;

/// <summary>
/// This class provides methods to simulate keyboard input.
/// </summary>
public class Keyboard :IKeyboard
{

    public void PressKey(KeyCode key)
    {
        Simulate.Events().Click(key).Invoke().Wait();
    }

    public void KeyDown(KeyCode key)
    {
        Simulate.Events().Hold(key).Invoke().Wait();
    }

    public void KeyUp(KeyCode key)
    {
        Simulate.Events().Release(key).Invoke().Wait();
    }

    public void TypeText(string text)
    {
        Simulate.Events().Click(text).Invoke().Wait();
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
        var sim = Simulate.Events();

        foreach (var key in Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>())
        {
            sim.Release(key);
        }

        sim.Invoke().Wait();
    }
}
