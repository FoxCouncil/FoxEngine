internal partial class KitchenSink : FoxEngine
{
    void DrawMainMenu()
    {
        DrawString(5, 25, "Hello from MainMenu", Pixel.White);
        DrawString(5, 45, DateTime.Now.ToString("G"), Pixel.White);
    }
}