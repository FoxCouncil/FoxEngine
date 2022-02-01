namespace FoxEngineLib.Types.Input;

public class ControlButton
{
    public bool Pressed;
    public bool Released;
    public bool Held;

    public override string ToString()
    {
        return $"Button({Pressed}, {Released}, {Held})";
    }
}
