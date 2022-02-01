namespace FoxEngineLib.Types.Drawing;
public interface IDrawable
{
    ulong ID { get; }

    void Draw(FoxEngine engine);
}
