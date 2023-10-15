namespace FoxEngineLib;

public abstract partial class FoxEngine
{
    public PixelMode PixelMode { get; set; } = PixelMode.NORMAL;

    public Sprite DrawTarget { get; private set; }

    public void Clear(Pixel clearColor)
    {
        if (DrawTarget != null)
        {
            DrawTarget.Clear(clearColor);
        }
    }

    public void DrawSpritePartial(int x, int y, Sprite sprite, int ox, int oy, int width, int height, int scale)
    {
        if (sprite == null)
        {
            return;
        }

        if (scale > 1)
        {
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    for (var @is = 0; @is < scale; @is++)
                    {
                        for (int js = 0; js < scale; js++)
                        {
                            Draw(x + (i * scale) + @is, y + (j * scale) + js, sprite.GetPixel(i + ox, j + oy));
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Draw(x + i, y + j, sprite.GetPixel(i + ox, j + oy));
                }
            }
        }
    }

    public void DrawSprite(int x, int y, Sprite sprite, int scale)
    {
        if (sprite == null)
        {
            return;
        }

        if (scale > 1)
        {
            for (int i = 0; i < sprite.Width; i++)
            {
                for (int j = 0; j < sprite.Height; j++)
                {
                    for (int @is = 0; @is < scale; @is++)
                    {
                        for (int js = 0; js < scale; js++)
                        {
                            Draw(x + (i * scale) + @is, y + (j * scale) + js, sprite.GetPixel(i, j));
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < sprite.Width; i++)
            {
                for (int j = 0; j < sprite.Height; j++)
                {
                    Draw(x + i, y + j, sprite.GetPixel(i, j));
                }
            }
        }
    }

    public void DrawCircleFilled(int x, int y, int r, Pixel pixel)
    {
        // Taken from wikipedia
        var x0 = 0;
        var y0 = r;
        var d = 3 - 2 * r;

        if (r == 0)
        {
            return;
        }

        void drawline(int sx, int ex, int ny) { for (int i = sx; i <= ex; i++) { Draw(i, ny, pixel); } }

        while (y0 >= x0)
        {
            // Modified to draw scan-lines instead of edges
            drawline(x - x0, x + x0, y - y0);
            drawline(x - y0, x + y0, y - x0);
            drawline(x - x0, x + x0, y + y0);
            drawline(x - y0, x + y0, y + x0);

            if (d < 0)
            {
                d += 4 * x0++ + 6;
            }
            else
            {
                d += 4 * (x0++ - y0--) + 10;
            }
        }
    }

    public void DrawCircle(int x, int y, int r, Pixel pixel, ushort mask = 0xFF)
    {
        var x0 = 0;
        var y0 = r;
        var d = 3 - 2 * r;

        if (r == 0)
        {
            return;
        }

        while (y0 >= x0) // only formulate 1/8 of circle
        {
            if (Convert.ToBoolean(mask & 0x01))
            {
                Draw(x + x0, y - y0, pixel);
            }

            if (Convert.ToBoolean(mask & 0x02))
            {
                Draw(x + y0, y - x0, pixel);
            }

            if (Convert.ToBoolean(mask & 0x04))
            {
                Draw(x + y0, y + x0, pixel);
            }

            if (Convert.ToBoolean(mask & 0x08))
            {
                Draw(x + x0, y + y0, pixel);
            }

            if (Convert.ToBoolean(mask & 0x10))
            {
                Draw(x - x0, y + y0, pixel);
            }

            if (Convert.ToBoolean(mask & 0x20))
            {
                Draw(x - y0, y + x0, pixel);
            }

            if (Convert.ToBoolean(mask & 0x40))
            {
                Draw(x - y0, y - x0, pixel);
            }

            if (Convert.ToBoolean(mask & 0x80))
            {
                Draw(x - x0, y - y0, pixel);
            }

            if (d < 0)
            {
                d += 4 * x0++ + 6;
            }
            else
            {
                d += 4 * (x0++ - y0--) + 10;
            }
        }
    }

    public void DrawTriangleFilled(int x1, int y1, int x2, int y2, int x3, int y3, Pixel p)
    {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
        void swap(object a, object b) { (a, b) = (b, a); }
#pragma warning restore IDE0059 // Unnecessary assignment of a value
        void drawline(int sx, int ex, int ny) { for (int i = sx; i <= ex; i++) { Draw(i, ny, p); } }

        var changed1 = false;
        var changed2 = false;

        int t1x, t2x, y, minx, maxx, t1xp, t2xp;
        int signx1, signx2, dx1, dy1, dx2, dy2;
        int e1, e2;

        // Sort vertices
        if (y1 > y2)
        {
            swap(y1, y2);
            swap(x1, x2);
        }

        if (y1 > y3)
        {
            swap(y1, y3);
            swap(x1, x3);
        }

        if (y2 > y3)
        {
            swap(y2, y3);
            swap(x2, x3);
        }

        // Starting points
        t1x = t2x = x1;
        y = y1;

        dx1 = x2 - x1;

        if (dx1 < 0)
        {
            dx1 = -dx1;
            signx1 = -1;
        }
        else
        {
            signx1 = 1;
        }

        dy1 = y2 - y1;
        dx2 = x3 - x1;

        if (dx2 < 0)
        {
            dx2 = -dx2;
            signx2 = -1;
        }
        else
        {
            signx2 = 1;
        }

        dy2 = y3 - y1;

        // swap values
        if (dy1 > dx1)
        {
            swap(dx1, dy1);
            changed1 = true;
        }

        // swap values
        if (dy2 > dx2)
        {
            swap(dy2, dx2);
            changed2 = true;
        }

        e2 = dx2 >> 1;

        // Flat top, just process the second half
        if (y1 == y2)
        {
            goto next;
        }

        e1 = dx1 >> 1;

        for (int i = 0; i < dx1;)
        {
            t1xp = 0;
            t2xp = 0;

            if (t1x < t2x)
            {
                minx = t1x;
                maxx = t2x;
            }
            else
            {
                minx = t2x; maxx = t1x;
            }

            // process first line until y value is about to change
            while (i < dx1)
            {
                i++;
                e1 += dy1;

                while (e1 >= dx1)
                {
                    e1 -= dx1;

                    if (changed1)
                    {
                        t1xp = signx1; //t1x += signx1;
                    }
                    else
                    {
                        goto next1;
                    }
                }

                if (changed1)
                {
                    break;
                }
                else
                {
                    t1x += signx1;
                }
            }

        // Move line
        next1:

            // process second line until y value is about to change
            while (true)
            {
                e2 += dy2;

                while (e2 >= dx2)
                {
                    e2 -= dx2;

                    if (changed2)
                    {
                        t2xp = signx2;//t2x += signx2;
                    }
                    else
                    {
                        goto next2;
                    }
                }

                if (changed2)
                {
                    break;
                }
                else
                {
                    t2x += signx2;
                }
            }

        next2:

            if (minx > t1x)
            {
                minx = t1x;
            }

            if (minx > t2x)
            {
                minx = t2x;
            }

            if (maxx < t1x)
            {
                maxx = t1x;
            }

            if (maxx < t2x)
            {
                maxx = t2x;
            }

            // Draw line from min to max points found on the y
            drawline(minx, maxx, y);

            // Now increase y
            if (!changed1)
            {
                t1x += signx1;
            }

            t1x += t1xp;

            if (!changed2)
            {
                t2x += signx2;
            }

            t2x += t2xp;
            y += 1;

            if (y == y2)
            {
                break;
            }
        }

    // Second half
    next:

        dx1 = x3 - x2;

        if (dx1 < 0)
        {
            dx1 = -dx1;
            signx1 = -1;
        }
        else
        {
            signx1 = 1;
        }

        dy1 = y3 - y2;
        t1x = x2;

        // swap values
        if (dy1 > dx1)
        {
            swap(dy1, dx1);
            changed1 = true;
        }
        else
        {
            changed1 = false;
        }

        e1 = dx1 >> 1;

        for (int i = 0; i <= dx1; i++)
        {
            t1xp = 0;
            t2xp = 0;

            if (t1x < t2x)
            {
                minx = t1x;
                maxx = t2x;
            }
            else
            {
                minx = t2x;
                maxx = t1x;
            }

            // process first line until y value is about to change
            while (i < dx1)
            {
                e1 += dy1;

                while (e1 >= dx1)
                {
                    e1 -= dx1;

                    if (changed1)
                    {
                        t1xp = signx1;
                        break;
                    }
                    else
                    {
                        goto next3;
                    }
                }

                if (changed1)
                {
                    break;
                }
                else
                {
                    t1x += signx1;
                }

                if (i < dx1)
                {
                    i++;
                }
            }

        // process second line until y value is about to change
        next3:

            while (t2x != x3)
            {
                e2 += dy2;

                while (e2 >= dx2)
                {
                    e2 -= dx2;

                    if (changed2)
                    {
                        t2xp = signx2;
                    }
                    else
                    {
                        goto next4;
                    }
                }

                if (changed2)
                {
                    break;
                }
                else
                {
                    t2x += signx2;
                }
            }

        next4:

            if (minx > t1x)
            {
                minx = t1x;
            }

            if (minx > t2x)
            {
                minx = t2x;
            }

            if (maxx < t1x)
            {
                maxx = t1x;
            }

            if (maxx < t2x)
            {
                maxx = t2x;
            }

            drawline(minx, maxx, y);

            if (!changed1)
            {
                t1x += signx1;
            }

            t1x += t1xp;

            if (!changed2)
            {
                t2x += signx2;
            }

            t2x += t2xp;
            y += 1;

            if (y > y3)
            {
                return;
            }
        }
    }

    public void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Pixel pixel)
    {
        DrawLine(x1, y1, x2, y2, pixel);
        DrawLine(x2, y2, x3, y3, pixel);
        DrawLine(x3, y3, x1, y1, pixel);
    }

    public void DrawRectFilled(int x, int y, int width, int height, Pixel pixel)
    {
        var x2 = x + width;
        var y2 = y + height;

        if (x < 0)
        {
            x = 0;
        }
        else if (x >= Resolution.Width)
        {
            x = Resolution.Width;
        }

        if (y < 0)
        {
            y = 0;
        }
        else if (y >= Resolution.Height)
        {
            y = Resolution.Height;
        }

        if (x2 < 0)
        {
            x2 = 0;
        }
        else if (x2 >= Resolution.Width)
        {
            x2 = Resolution.Width;
        }

        if (y2 < 0)
        {
            y2 = 0;
        }
        else if (y2 >= Resolution.Height)
        {
            y2 = Resolution.Height;
        }

        for (int i = x; i < x2; i++)
        {
            for (int j = y; j < y2; j++)
            {
                Draw(i, j, pixel);
            }
        }
    }

    public void DrawRect(int x, int y, int width, int height, Pixel pixel)
    {
        DrawLine(x, y, x + width, y, pixel); // Works
        DrawLine(x + width, y, x + width, y + height, pixel);
        DrawLine(x + width, y + height, x, y + height, pixel);
        DrawLine(x, y + height, x, y, pixel);
    }

    public void DrawLine(int x1, int y1, int x2, int y2, Pixel pixel)
    {
        int x, y, dx1, dy1, px, py, xe, ye, i;

        var dx = x2 - x1;

        if (dx == 0)
        {
            if (y2 < y1)
            {
                (y2, y1) = (y1, y2);
            }

            for (y = y1; y <= y2; y++)
            {
                Draw(x1, y, pixel);
            }

            return;
        }

        var dy = y2 - y1;

        if (dy == 0)
        {
            if (x2 < x1)
            {
                (x2, x1) = (x1, x2);
            }

            for (x = x1; x <= x2; x++)
            {
                Draw(x, y1, pixel);
            }

            return;
        }

        dx1 = Math.Abs(dx);
        dy1 = Math.Abs(dy);

        px = 2 * dy1 - dx1;
        py = 2 * dx1 - dy1;

        if (dy1 <= dx1)
        {
            if (dx >= 0)
            {
                x = x1;
                y = y1;
                xe = x2;
            }
            else
            {
                x = x2;
                y = y2;
                xe = x1;
            }

            Draw(x, y, pixel);

            for (i = 0; x < xe; i++)
            {
                x += 1;

                if (px < 0)
                {
                    px += 2 * dy1;
                }
                else
                {
                    if ((dx < 0 && dy < 0) || (dx > 0 && dy > 0))
                    {
                        y += 1;
                    }
                    else
                    {
                        y -= 1;
                    }

                    px += 2 * (dy1 - dx1);
                }

                Draw(x, y, pixel);
            }
        }
        else
        {
            if (dy >= 0)
            {
                x = x1;
                y = y1;
                ye = y2;
            }
            else
            {
                x = x2;
                y = y2;
                ye = y1;
            }

            Draw(x, y, pixel);

            for (i = 0; y < ye; i++)
            {
                y += 1;

                if (py <= 0)
                {
                    py += 2 * dx1;
                }
                else
                {
                    if ((dx < 0 && dy < 0) || (dx > 0 && dy > 0))
                    {
                        x += 1;
                    }
                    else
                    {
                        x -= 1;
                    }

                    py += 2 * (dx1 - dy1);
                }

                Draw(x, y, pixel);
            }
        }
    }

    public void DrawString(int x, int y, string text, Pixel pixel, int scale = 1)
    {
        var sx = 0;
        var sy = 0;
        var pm = PixelMode;

        if (pixel.A != 255)
        {
            PixelMode = PixelMode.ALPHA;
        }
        else
        {
            PixelMode = PixelMode.MASK;
        }

        foreach (var c in text)
        {
            if (c == '\n')
            {
                sx = 0;
                sy += 8 * scale;
            }
            else
            {
                var ox = (c - 32) % 16;
                var oy = (c - 32) / 16;

                for (var i = 0; i < 8; i++)
                {
                    for (var j = 0; j < 8; j++)
                    {
                        if (_fontSprite.GetPixel(i + ox * 8, j + oy * 8).R > 0)
                        {
                            if (scale > 1)
                            {
                                for (var @is = 0; @is < scale; @is++)
                                {
                                    for (var js = 0; js < scale; js++)
                                    {
                                        Draw(x + sx + (i * scale) + @is, y + sy + (j * scale) + js, pixel);
                                    }
                                }
                            }
                            else
                            {
                                Draw(x + sx + i, y + sy + j, pixel);
                            }
                        }
                    }
                }

                sx += 8 * scale;
            }
        }

        PixelMode = pm;
    }

    [DebuggerStepThrough]
    public void Draw(int x, int y, Pixel pixel)
    {
        if (DrawTarget == null)
        {
            return;
        }

        if (PixelMode == PixelMode.NORMAL)
        {
            DrawTarget.SetPixel(x, y, pixel);
        }
        else if (PixelMode == PixelMode.MASK)
        {
            if (pixel.A == 255)
            {
                DrawTarget.SetPixel(x, y, pixel);
            }
        }
        else if (PixelMode == PixelMode.ALPHA)
        {
            var origPixel = DrawTarget.GetPixel(x, y);
            var alpha = pixel.A / 255.0f * 1;
            var cof = 1.0f - alpha;
            var red = alpha * pixel.R + cof * origPixel.R;
            var green = alpha * pixel.G + cof * origPixel.G;
            var blue = alpha * pixel.B + cof * origPixel.B;

            DrawTarget.SetPixel(x, y, new Pixel(Convert.ToByte(red), Convert.ToByte(green), Convert.ToByte(blue)));
        }
    }
}