namespace FoxEngineLib.Types.Drawing;

public class Sprite
{
    // private const int kBpp = 4;

    public Pixel[] PixelData;

    public int Width { get; }

    public int Height { get; }

    public int TotalPixels { get; }

    public Sprite()
    {
        Width = 20;
        Height = 20;

        TotalPixels = Width * Height;

        PixelData = new Pixel[TotalPixels];
    }

    public Sprite(int width, int height)
    {
        Width = width;
        Height = height;

        TotalPixels = Width * Height;

        PixelData = new Pixel[TotalPixels];

        for (var idx = 0; idx < TotalPixels; idx++)
        {
            PixelData[idx] = Pixel.White;
        }
    }

    public Sprite(Size size) : this(size.Width, size.Height) { }

    public Pixel GetPixel(int x, int y)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            return PixelData[y * Width + x];
        }

        return Pixel.Blank;
    }

    public void SetPixel(int x, int y, Pixel pixel)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            PixelData[y * Width + x] = pixel;
        }
    }

    public void Clear(Pixel color)
    {
        for (var idx = 0; idx < TotalPixels; idx++)
        {
            PixelData[idx] = color;
        }
    }
}
