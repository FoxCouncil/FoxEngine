namespace FoxEngineLib.Types.UI;

public abstract class Control : IDrawable
{
    public ulong ID { get; private set; }

    public Rectangle Bounds { get; set; } = Rectangle.FromLTRB(0, 0, 10, 40);

    public Control(int x, int y, int width, int height)
    {
        ID = FoxEngine.GetControlIndex();

        Bounds = new Rectangle(x, y, width, height);
    }

    public abstract void Draw(FoxEngine engine);
}
