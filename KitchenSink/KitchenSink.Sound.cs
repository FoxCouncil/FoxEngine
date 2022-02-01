using FoxEngineLib.Types;
using System.Drawing;

internal partial class KitchenSink : FoxEngine
{
    void DrawSoundTest()
    {
        DrawString(5, 25, $"BEEP BEEP", Pixel.White);
    }
}