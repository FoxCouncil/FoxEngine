namespace FoxEngineLib;

public abstract partial class FoxEngine
{
    public static Dictionary<KeyboardButton, ControlButton> Keyboard = new();

    public static Dictionary<MouseButton, ControlButton> Mouse = new();

    public static readonly Point MOUSE_OUTSIDE = new(-1, -1);

    public static Point RealMousePosition;

    public static Point MousePosition;

    public bool IsHovered { get; internal set; }

    public bool IsFocused { get; internal set; } = true;

    static void GenerateUserInput()
    {
        for (var keyIdx = 0; keyIdx < (int)KeyboardButton.MAX; keyIdx++)
        {
            Keyboard.Add((KeyboardButton)keyIdx, new ControlButton());
        }

        for (var mbIdx = 0; mbIdx < (int)MouseButton.MAX; mbIdx++)
        {
            Mouse.Add((MouseButton)mbIdx, new ControlButton());
        }
    }
}
