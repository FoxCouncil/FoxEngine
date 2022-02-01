namespace FoxEngineLib.Types.UI
{
    public class Button : Control
    {
        public Pixel BackgroundColor { get; set; } = Pixel.White;

        public Pixel ForegroundColor { get; set; } = Pixel.Black;

        public bool EnabledHoveredState { get; set; } = true;

        public string Text { get; set; } = "Button";

        public event EventHandler? OnClick;

        public Button(int x, int y, int width, int height, string text) : base(x, y, width, height)
        {
            Text = text;
        }

        public override void Draw(FoxEngine engine)
        {
            var hovered = FoxEngine.MousePosition != FoxEngine.MOUSE_OUTSIDE && Bounds.Contains(FoxEngine.MousePosition);

            engine.DrawRectFilled(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, hovered ? ForegroundColor : BackgroundColor);
            engine.DrawRect(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, hovered ? BackgroundColor : ForegroundColor);
            engine.DrawString(Bounds.X + 10, Bounds.Y + (Bounds.Height / 3), Text, hovered ? BackgroundColor : ForegroundColor);

            if (hovered && FoxEngine.Mouse[MouseButton.Left].Pressed)
            {
                OnClick?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
