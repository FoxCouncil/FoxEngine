using FoxEngineLib.Types;
using FoxEngineLib.Types.Input;
using static FoxEngineLib.Types.Input.KeyboardButton;
using System.Drawing;

internal partial class KitchenSink : FoxEngine
{
    readonly KeyboardButton[,] _keyMap = new KeyboardButton[,]
    {
        { Escape, None, F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12, PrintScreen, ScrollLock, Pause, None, None, None, None },
        { Backtick, One, Two, Three, Four, Five, Six, Seven, Eight, Nine, Zero, Minus, Plus, Backspace, Insert, Home, PageUp, NumLock, NumpadDivide, NumpadMultiply, NumpadSubtract },
        { Tab, Q, W, E, R, T, Y, U, I, O, P, SquareBracketOpen, SquareBracketClosed, BackSlash, Delete, End, PageDown, NumpadSeven, NumpadEight, NumpadNine, NumpadAdd },
        { CapsLock, A, S, D, F, G, H, J, K, L, Semicolon, Quote, Enter, NumpadFour, NumpadFive, NumpadSix, None, None, None, None, None },
        { LeftShift, Z, X, C, V, B, N, M, Comma, Period, ForwardSlash, RightShift, Up, NumpadOne, NumpadTwo, NumpadThree, NumpadEnter, None, None, None, None },
        { LeftControl, LeftWindows, LeftAlt, Space, RightAlt, RightWindows, Apps, RightControl, Left, Down, Right, NumpadZero, NumpadDecimal, None, None, None, None, None, None, None, None }
    };

    Pixel GetButtonBackgroundColor(int x, int y)
    {
        if (x < _keyMap.GetLength(0) && y < _keyMap.GetLength(1))
        {
            var aKey = Keyboard.First(key => key.Key == _keyMap[x, y]);

            return (aKey.Value.Pressed || aKey.Value.Held || aKey.Value.Released) ? Pixel.Green : Pixel.Black;
        }

        return Pixel.Black;
    }

    string GetButtonString(int x, int y)
    {
        if (x < _keyMap.GetLength(0) && y < _keyMap.GetLength(1))
        {
            var keyboardButton = _keyMap[x, y];
            return ((int)keyboardButton).ToString("x").PadLeft(2, '0').ToUpper();
        }

        return "-";
    }

    int GetButtonStringOffset(int len)
    {
        if (len == 1)
        {
            return 7;
        }
        
        if (len == 2)
        {
            return 3;
        }

        return -4 * (len - 2);
    }

    void DrawControlTest()
    {
        DrawString(5, 25, $"         MousePos: {RealMousePosition.X},{RealMousePosition.Y}", Pixel.White);

        DrawString(5, 55, $"Relative MousePos: {MousePosition.X},{MousePosition.Y}", Pixel.White);

        var keysDown = Keyboard.Where(x => x.Value.Pressed || x.Value.Held || x.Value.Released);

        var y = 55 + 30;

        var rightSideOffset = OriginalResolution.Width - 10;

        for (var i = 0; i < 7; i++)
        {
            var x = 6;

            if (i == 0)
            {
                for (var j = 0; j < 17; j++)
                {
                    var keyStr = GetButtonString(i, j);
                    var offset = GetButtonStringOffset(keyStr.Length);

                    if (j != 1)
                    {
                        DrawRectFilled(x, y, 20, 20, GetButtonBackgroundColor(i, j));
                        DrawString(x + offset, y + 7, keyStr, Pixel.White);
                    }

                    if (j == 5 || j == 9)
                    {
                        x += 33;
                    }
                    else if (j == 13)
                    {
                        x += 27;
                    }
                    else
                    {
                        x += 22;
                    }
                }
            }
            else if (i == 1)
            {
                for (var j = 0; j < 21; j++)
                {
                    var keyStr = GetButtonString(i, j);
                    var offset = GetButtonStringOffset(keyStr.Length);

                    if (j == 13)
                    {
                        DrawRectFilled(x, y, 42, 20, GetButtonBackgroundColor(i, j));
                    }
                    else
                    {
                        DrawRectFilled(x, y, 20, 20, GetButtonBackgroundColor(i, j));
                    }

                    DrawString(x + offset, y + 7, keyStr, Pixel.White);

                    if (j == 13)
                    {
                        x += 49;
                    }
                    else if (j == 16)
                    {
                        x += 27;
                    }
                    else
                    {
                        x += 22;
                    }
                }
            }
            else if (i == 2)
            {
                for (var j = 0; j < 21; j++)
                {
                    var keyStr = GetButtonString(i, j);
                    var offset = GetButtonStringOffset(keyStr.Length);

                    if (j == 0 || j == 13)
                    {
                        DrawRectFilled(x, y, 31, 20, GetButtonBackgroundColor(i, j));
                    }
                    else if (j == 20)
                    {
                        DrawRectFilled(x, y, 20, 42, GetButtonBackgroundColor(i, j));
                    }
                    else
                    {
                        DrawRectFilled(x, y, 20, 20, GetButtonBackgroundColor(i, j));
                    }

                    DrawString(x + offset, y + 7, keyStr, Pixel.White);

                    if (j == 0)
                    {
                        x += 33;
                    }
                    else if (j == 13)
                    {
                        x += 38;
                    }
                    else if (j == 16)
                    {
                        x += 27;
                    }
                    else
                    {
                        x += 22;
                    }
                }
            }
            else if (i == 3)
            {
                for (var j = 0; j < 16; j++)
                {
                    var keyStr = GetButtonString(i, j);
                    var offset = GetButtonStringOffset(keyStr.Length);

                    if (j == 0 || j == 12)
                    {
                        DrawRectFilled(x, y, 42, 20, GetButtonBackgroundColor(i, j));
                    }
                    else
                    {
                        DrawRectFilled(x, y, 20, 20, GetButtonBackgroundColor(i, j));
                    }

                    DrawString(x + offset, y + 7, keyStr, Pixel.White);

                    if (j == 0)
                    {
                        x += 44;
                    }
                    else if (j == 12)
                    {
                        x += 120;
                    }
                    else if (j == 15)
                    {
                        x += 27;
                    }
                    else
                    {
                        x += 22;
                    }
                }
            }
            else if (i == 4)
            {
                for (var j = 0; j < 17; j++)
                {
                    var keyStr = GetButtonString(i, j);
                    var offset = GetButtonStringOffset(keyStr.Length);

                    if (j == 0 || j == 11)
                    {
                        DrawRectFilled(x, y, 53, 20, GetButtonBackgroundColor(i, j));
                    }
                    else if (j == 16)
                    {
                        DrawRectFilled(x, y, 20, 42, GetButtonBackgroundColor(i, j));
                    }
                    else
                    {
                        DrawRectFilled(x, y, 20, 20, GetButtonBackgroundColor(i, j));
                    }

                    DrawString(x + offset, y + 7, keyStr, Pixel.White);

                    if (j == 0)
                    {
                        x += 55;
                    }
                    else if (j == 11)
                    {
                        x += 82;
                    }
                    else if (j == 12)
                    {
                        x += 49;
                    }
                    else
                    {
                        x += 22;
                    }
                }
            }
            else if (i == 5)
            {
                for (var j = 0; j < 13; j++)
                {
                    var keyStr = GetButtonString(i, j);
                    var offset = GetButtonStringOffset(keyStr.Length);

                    var spacebarWidth = 134;

                    if (j == 0 || j == 2 || j == 4 || j == 7)
                    {
                        DrawRectFilled(x, y, 30, 20, GetButtonBackgroundColor(i, j));
                    }
                    else if (j == 3) // Spacebar!
                    {
                        DrawRectFilled(x, y, spacebarWidth, 20, GetButtonBackgroundColor(i, j));
                    }
                    else if (j == 11) // 0 ins
                    {
                        DrawRectFilled(x, y, 42, 20, GetButtonBackgroundColor(i, j));
                    }
                    else
                    {
                        DrawRectFilled(x, y, 20, 20, GetButtonBackgroundColor(i, j));
                    }

                    DrawString(x + offset, y + 7, keyStr, Pixel.White);

                    if (j == 0 || j == 2 || j == 4)
                    {
                        x += 32;
                    }
                    else if (j == 3)
                    {
                        x += spacebarWidth + 2;
                    }
                    else if (j == 7)
                    {
                        x += 37;
                    }
                    else if (j == 10)
                    {
                        x += 27;
                    }
                    else if (j == 11)
                    {
                        x += 44;
                    }
                    else
                    {
                        x += 22;
                    }
                }
            }

            y += i == 0 ? 25 : 22;
        }

        foreach (var key in keysDown)
        {
            DrawString(5, y, key.Key.ToString(), Pixel.White);

            y += 30;
        }
    }
}