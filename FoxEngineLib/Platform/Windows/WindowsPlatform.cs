using System.ComponentModel;

using static FoxEngineLib.Platform.Windows.WindowsInterop;
using static FoxEngineLib.Platform.Windows.WindowsTypes;
using static FoxEngineLib.Types.Input.KeyboardButton;

namespace FoxEngineLib.Platform.Windows;

internal class WindowsPlatform : IPlatform
{
    const string WINDOW_CLASS_NAME = "WC_FOX_ENGINE";
    const string WINDOWS_NOT_SUPPORTED_MESSAGE = "This program requires Windows NT!";

    readonly FoxEngine _engine;

    readonly bool[] _keyboardStateNew;
    readonly bool[] _keyboardStateOld;

    readonly bool[] _mouseStateNew;
    readonly bool[] _mouseStateOld;

    Point _realMousePositionState = new();

    WNDCLASSEX _windowClass;

    TRACKMOUSEEVENT _trackMouseEvent;

    IntPtr _windowHandle;
    IntPtr _deviceContext;
    IntPtr _renderingContext;

    GLuint _glBuffer;

    internal WindowsPlatform(FoxEngine engine)
    {
        _keyboardStateNew = new bool[(int)KeyboardButton.MAX];
        _keyboardStateOld = new bool[(int)KeyboardButton.MAX];

        _mouseStateNew = new bool[(int)MouseButton.MAX];
        _mouseStateOld = new bool[(int)MouseButton.MAX];

        _engine = engine;
    }

    public void MessageBox(string title, string message)
    {
        _ = WindowsInterop.MessageBox(IntPtr.Zero, message, title, 0);
    }

    public void Initialize()
    {
        CreateWindowClass();

        if (_windowHandle != IntPtr.Zero)
        {
            throw new ApplicationException("Window handle already created!");
        }

        var windowStyleEx = WindowStylesEx.WS_EX_APPWINDOW | WindowStylesEx.WS_EX_WINDOWEDGE;
        var windowStyle = WindowStyles.WS_CLIPCHILDREN | WindowStyles.WS_CLIPSIBLINGS | WindowStyles.WS_OVERLAPPEDWINDOW;
        var windowRect = new RECT(0, 0, _engine.Resolution.Width, _engine.Resolution.Height);

        AdjustWindowRect(ref windowRect, windowStyle, false);

        _windowHandle = CreateWindowEx(windowStyleEx, WINDOW_CLASS_NAME, _engine.Name, windowStyle, 0, 0, windowRect.Width, windowRect.Height, IntPtr.Zero, IntPtr.Zero, GetModuleHandle(null), IntPtr.Zero);

        if (_windowHandle == IntPtr.Zero)
        {
            int lastError = Marshal.GetLastWin32Error();
            string errorMessage = new Win32Exception(lastError).Message;

            throw new Exception(errorMessage);
        }

        _trackMouseEvent = new TRACKMOUSEEVENT
        {
            cbSize = Marshal.SizeOf(typeof(TRACKMOUSEEVENT)),
            dwFlags = (uint)TMEFlags.TME_LEAVE,
            hwndTrack = _windowHandle
        };

        ShowWindow(_windowHandle, ShowWindowCommands.Normal);
        UpdateWindow(_windowHandle);
    }

    public void Run()
    {
        int returnVal;

        while ((returnVal = GetMessage(out MSG _windowMsg, IntPtr.Zero, 0, 0)) != 0)
        {
            if (returnVal < 0)
            {
                throw new Exception("Woah");
            }
            else
            {
                TranslateMessage(ref _windowMsg);
                DispatchMessage(ref _windowMsg);
            }
        }
    }

    public void Dispose()
    {
        wglDeleteContext(_deviceContext);

        PostMessage(_windowHandle, (uint)WindowsMessages.DESTROY, IntPtr.Zero, IntPtr.Zero);
    }

    public void CreateGlContext()
    {
        CreateDeviceContext();

        var pixelFormat = GetBasicPixelFormatDescriptor();

        var pf = ChoosePixelFormat(_deviceContext, ref pixelFormat);

        if (pf == 0)
        {
            ProcessWin32Error();
        }

        if (!SetPixelFormat(_deviceContext, pf, ref pixelFormat))
        {
            ProcessWin32Error();
        }

        _renderingContext = wglCreateContext(_deviceContext);

        if (_renderingContext == IntPtr.Zero)
        {
            ProcessWin32Error();
        }

        wglMakeCurrent(_deviceContext, _renderingContext);

        glViewport(0, 0, _engine.Resolution.Width, _engine.Resolution.Height);

        var wglSwapIntervalPtr = wglGetProcAddress("wglSwapIntervalEXT");

        if (wglSwapIntervalPtr != IntPtr.Zero)
        {
            var wglSwapInterval = Marshal.GetDelegateForFunctionPointer<wglSwapInterval>(wglSwapIntervalPtr);

            wglSwapInterval(0);
        }

        glEnable(GL_TEXTURE_2D);

        glGenTextures(1, ref _glBuffer);

        glBindTexture(GL_TEXTURE_2D, _glBuffer);

        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, (int)GL_NEAREST);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, (int)GL_NEAREST);

        glTexEnvf(GL_TEXTURE_ENV, GL_TEXTURE_ENV_MODE, GL_DECAL);

        glTexImage2D(GL_TEXTURE_2D, 0, (int)GL_RGBA, _engine.DrawTarget.Width, _engine.DrawTarget.Height, 0, GL_RGBA, GL_UNSIGNED_BYTE, IntPtr.Zero);
    }

    public void Draw()
    {
        if (!_engine.IsRunning)
        {
            return;
        }

        for (var keyIdx = 0; keyIdx < (int)KeyboardButton.MAX; keyIdx++)
        {
            var currentKey = FoxEngine.Keyboard[(KeyboardButton)keyIdx];

            currentKey.Pressed = false;
            currentKey.Released = false;

            if (_keyboardStateNew[keyIdx] != _keyboardStateOld[keyIdx])
            {
                if (_keyboardStateNew[keyIdx])
                {
                    currentKey.Pressed = !currentKey.Held;
                    currentKey.Held = true;
                }
                else
                {
                    currentKey.Released = true;
                    currentKey.Held = false;
                }

                _keyboardStateOld[keyIdx] = _keyboardStateNew[keyIdx];
            }
        }

        TrackMouseEvent(ref _trackMouseEvent);

        FoxEngine.RealMousePosition = _realMousePositionState;

        if (_realMousePositionState == FoxEngine.MOUSE_OUTSIDE)
        {
            _engine.IsHovered = false;

            FoxEngine.MousePosition = FoxEngine.MOUSE_OUTSIDE;
        }
        else
        {
            _engine.IsHovered = true;

            var logicalMousePos = new Point(_realMousePositionState.X.RangeConvert(0, _engine.Resolution.Width, 0, _engine.OriginalResolution.Width), _realMousePositionState.Y.RangeConvert(0, _engine.Resolution.Height, 0, _engine.OriginalResolution.Height));

            FoxEngine.MousePosition = _realMousePositionState == FoxEngine.MOUSE_OUTSIDE ? FoxEngine.MOUSE_OUTSIDE : logicalMousePos;
        }

        for (var mbIdx = 0; mbIdx < (int)MouseButton.MAX; mbIdx++)
        {
            var mouseButton = FoxEngine.Mouse[(MouseButton)mbIdx];

            mouseButton.Pressed = false;
            mouseButton.Released = false;

            if (_mouseStateNew[mbIdx] != _mouseStateOld[mbIdx])
            {
                if (_mouseStateNew[mbIdx])
                {
                    mouseButton.Pressed = !mouseButton.Held;
                    mouseButton.Held = true;
                }
                else
                {
                    mouseButton.Released = true;
                    mouseButton.Held = false;
                }

                _mouseStateOld[mbIdx] = _mouseStateNew[mbIdx];
            }
        }

        glViewport(0, 0, _engine.Resolution.Width, _engine.Resolution.Height);

        glClear(GL_COLOR_BUFFER_BIT);

        var handle = GCHandle.Alloc(_engine.DrawTarget.PixelData, GCHandleType.Pinned);

        glTexImage2D(GL_TEXTURE_2D, 0, (int)GL_RGBA, _engine.DrawTarget.Width, _engine.DrawTarget.Height, 0, GL_RGBA, GL_UNSIGNED_BYTE, handle.AddrOfPinnedObject());

        var error = glGetError();

        if (error != GL_NO_ERROR)
        {
            List<string> errors = new()
            {
                error.ToString()
            };

            error = glGetError();

            while (error != GL_NO_ERROR)
            {
                errors.Add(error.ToString());

                error = glGetError();
            }

            MessageBox("Error!", string.Join(", ", errors));
        }

        handle.Free();

        glBegin(GL_QUADS);

        glTexCoord2f(0f, 1f);
        glVertex3f(-1f, -1f, 0f);

        glTexCoord2f(0f, 0f);
        glVertex3f(-1f, 1f, 0f);

        glTexCoord2f(1f, 0f);
        glVertex3f(1f, 1f, 0f);

        glTexCoord2f(1f, 1f);
        glVertex3f(1f, -1f, 0);

        glEnd();

        SwapBuffers(_deviceContext);
    }

    public void SetWindowTitle(string title)
    {
        if (!_engine.IsRunning)
        {
            return;
        }

        SetWindowTextW(_windowHandle, $"{_engine.Name} - {title}");
    }

    void CreateWindowClass()
    {
        _windowClass = new WNDCLASSEX
        {
            hIcon = LoadIcon(IntPtr.Zero, SystemIcons.IDI_APPLICATION),
            hCursor = LoadCursor(IntPtr.Zero, SystemCursors.IDC_ARROW),
            hInstance = GetModuleHandle(string.Empty),
            lpfnWndProc = WindowMessage,
            cbSize = (uint)Marshal.SizeOf(typeof(WNDCLASSEX)),
            cbClsExtra = 0,
            cbWndExtra = 0,
            hbrBackground = IntPtr.Zero,
            lpszMenuName = string.Empty,
            lpszClassName = WINDOW_CLASS_NAME,
            style = ClassStyles.HorizontalRedraw | ClassStyles.VerticalRedraw | ClassStyles.OwnDC
        };

        var regResult = RegisterClassEx(ref _windowClass);

        if (regResult == 0)
        {
            _ = WindowsInterop.MessageBox(IntPtr.Zero, WINDOWS_NOT_SUPPORTED_MESSAGE, _engine.Name, MessageBoxOptions.IconError);
            throw new Exception(WINDOWS_NOT_SUPPORTED_MESSAGE);
        }
    }

    IntPtr WindowMessage(IntPtr hWnd, WindowsMessages msg, IntPtr wParam, IntPtr lParam)
    {
        switch (msg)
        {
            case WindowsMessages.KEYUP:
            {
                var v = wParam.ToInt32();

                var newV = (VK)v;

                var keyCode = _keyboardMap[newV];

                _keyboardStateNew[(int)keyCode] = false;

                if (keyCode == None)
                {
                    Debug.WriteLine($"  KeyUp({v}): {keyCode}, {keyCode}");
                }
            }
            break;

            case WindowsMessages.KEYDOWN:
            {
                var v = wParam.ToInt32();

                var newV = (VK)v;

                var keyCode = _keyboardMap[newV];

                _keyboardStateNew[(int)keyCode] = true;

                if (keyCode == None)
                {
                    Debug.WriteLine($"KeyDown({v}): {keyCode}, {keyCode}");
                }
            }
            break;

            case WindowsMessages.LBUTTONDOWN:
            {
                _mouseStateNew[(int)MouseButton.Left] = true;
            }
            break;

            case WindowsMessages.LBUTTONUP:
            {
                _mouseStateNew[(int)MouseButton.Left] = false;
            }
            break;

            case WindowsMessages.RBUTTONDOWN:
            {
                _mouseStateNew[(int)MouseButton.Right] = true;
            }
            break;

            case WindowsMessages.RBUTTONUP:
            {
                _mouseStateNew[(int)MouseButton.Right] = false;
            }
            break;

            case WindowsMessages.MBUTTONDOWN:
            {
                _mouseStateNew[(int)MouseButton.Middle] = true;
            }
            break;

            case WindowsMessages.MBUTTONUP:
            {
                _mouseStateNew[(int)MouseButton.Middle] = false;
            }
            break;

            case WindowsMessages.MOUSEMOVE:
            {
                var x = lParam.ToInt32() & 0xFFFF;
                var y = lParam.ToInt32() >> 16;

                _realMousePositionState = new Point(x, y);
            }
            break;

            case WindowsMessages.MOUSELEAVE:
            {
                _realMousePositionState = FoxEngine.MOUSE_OUTSIDE;
            }
            break;

            case WindowsMessages.SETFOCUS:
            {
                _engine.IsFocused = true;
            }
            break;

            case WindowsMessages.KILLFOCUS:
            {
                _engine.IsFocused = false;
            }
            break;

            case WindowsMessages.SIZE:
            {
                var width = lParam.ToInt32() & 0xFFFF;
                var height = lParam.ToInt32() >> 16;

                _engine.Resize(width, height);
            }
            break;

            case WindowsMessages.GETMINMAXINFO:
            {
                var mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);

                if (mmi == null)
                {
                    return DefWindowProc(hWnd, msg, wParam, lParam);
                }

                mmi.ptMinTrackSize.X = _engine.OriginalResolution.Width;
                mmi.ptMinTrackSize.Y = _engine.OriginalResolution.Height;

                Marshal.StructureToPtr(mmi, lParam, true);
            }
            break;

            case WindowsMessages.CLOSE:
            {
                _engine.Stop();
            }
            break;

            case WindowsMessages.DESTROY:
            {
                PostQuitMessage(0);
            }
            break;

            default:
            {
                return DefWindowProc(hWnd, msg, wParam, lParam);
            }
        }

        return IntPtr.Zero;
    }

    private void CreateDeviceContext()
    {
        _deviceContext = GetDC(_windowHandle);

        if (_deviceContext != IntPtr.Zero)
        {
            return;
        }

        var a_lastError = Marshal.GetLastWin32Error();
        throw new Exception(new Win32Exception(a_lastError).Message);
    }

    private static PIXELFORMATDESCRIPTOR GetBasicPixelFormatDescriptor()
    {
        PIXELFORMATDESCRIPTOR pfd;

        pfd.nSize = Convert.ToUInt16(Marshal.SizeOf<PIXELFORMATDESCRIPTOR>());
        pfd.nVersion = 1;
        pfd.dwFlags = PFD_DRAW_TO_WINDOW | PFD_SUPPORT_OPENGL | PFD_DOUBLEBUFFER;
        pfd.iPixelType = Convert.ToByte(PFD_TYPE_RGBA);
        pfd.cColorBits = 32;
        pfd.cRedBits = 0;
        pfd.cRedShift = 0;
        pfd.cGreenBits = 0;
        pfd.cGreenShift = 0;
        pfd.cBlueBits = 0;
        pfd.cBlueShift = 0;
        pfd.cAlphaBits = 0;
        pfd.cAlphaShift = 0;
        pfd.cAccumBits = 0;
        pfd.cAccumRedBits = 0;
        pfd.cAccumGreenBits = 0;
        pfd.cAccumBlueBits = 0;
        pfd.cAccumAlphaBits = 0;
        pfd.cDepthBits = 24;
        pfd.cStencilBits = 8;
        pfd.cAuxBuffers = 0;
        pfd.iLayerType = Convert.ToByte(PFD_MAIN_PLANE);
        pfd.bReserved = 0;
        pfd.dwLayerMask = 0;
        pfd.dwVisibleMask = 0;
        pfd.dwDamageMask = 0;

        return pfd;
    }

    private static void ProcessWin32Error()
    {
        int lastError = Marshal.GetLastWin32Error();
        throw new Win32Exception(lastError);
    }

    private readonly Dictionary<VK, KeyboardButton> _keyboardMap = new Dictionary<VK, KeyboardButton>
    {
        { VK.BACK, Backspace },
        { VK.TAB, Tab },
        { VK.CLEAR, Clear },
        { VK.RETURN, Enter },
        { VK.SHIFT, LeftShift },
        { VK.CONTROL, LeftControl },
        { VK.MENU, LeftAlt },
        { VK.PAUSE, Pause },
        { VK.CAPITAL, CapsLock },
        { VK.ESCAPE, Escape },
        { VK.SPACE, Space },
        { VK.PRIOR, PageUp },
        { VK.NEXT, PageDown },
        { VK.END, End },
        { VK.HOME, Home },
        { VK.LEFT, Left },
        { VK.UP, Up },
        { VK.RIGHT, Right },
        { VK.DOWN, Down },
        { VK.SNAPSHOT, PrintScreen },
        { VK.INSERT, Insert },
        { VK.DELETE, Delete },
        { VK.KEY_0, Zero },
        { VK.KEY_1, One },
        { VK.KEY_2, Two },
        { VK.KEY_3, Three },
        { VK.KEY_4, Four },
        { VK.KEY_5, Five },
        { VK.KEY_6, Six },
        { VK.KEY_7, Seven },
        { VK.KEY_8, Eight },
        { VK.KEY_9, Nine },
        { VK.KEY_A, A },
        { VK.KEY_B, B },
        { VK.KEY_C, C },
        { VK.KEY_D, D },
        { VK.KEY_E, E },
        { VK.KEY_F, F },
        { VK.KEY_G, G },
        { VK.KEY_H, H },
        { VK.KEY_I, I },
        { VK.KEY_J, J },
        { VK.KEY_K, K },
        { VK.KEY_L, L },
        { VK.KEY_M, M },
        { VK.KEY_N, N },
        { VK.KEY_O, O },
        { VK.KEY_P, P },
        { VK.KEY_Q, Q },
        { VK.KEY_R, R },
        { VK.KEY_S, S },
        { VK.KEY_T, T },
        { VK.KEY_U, U },
        { VK.KEY_V, V },
        { VK.KEY_W, W },
        { VK.KEY_X, X },
        { VK.KEY_Y, Y },
        { VK.KEY_Z, Z },
        { VK.LWIN, LeftWindows },
        { VK.RWIN, RightWindows },
        { VK.APPS, Apps },
        { VK.SLEEP, Sleep },
        { VK.NUMPAD0, NumpadZero },
        { VK.NUMPAD1, NumpadOne },
        { VK.NUMPAD2, NumpadTwo },
        { VK.NUMPAD3, NumpadThree },
        { VK.NUMPAD4, NumpadFour },
        { VK.NUMPAD5, NumpadFive },
        { VK.NUMPAD6, NumpadSix },
        { VK.NUMPAD7, NumpadSeven },
        { VK.NUMPAD8, NumpadEight },
        { VK.NUMPAD9, NumpadNine },
        { VK.MULTIPLY, NumpadMultiply },
        { VK.ADD, NumpadAdd },
        { VK.SUBTRACT, NumpadSubtract },
        { VK.DECIMAL, NumpadDecimal },
        { VK.DIVIDE, NumpadDivide },
        { VK.F1, F1 },
        { VK.F2, F2 },
        { VK.F3, F3 },
        { VK.F4, F4 },
        { VK.F5, F5 },
        { VK.F6, F6 },
        { VK.F7, F7 },
        { VK.F8, F8 },
        { VK.F9, F9 },
        { VK.F10, F10 },
        { VK.F11, F11 },
        { VK.F12, F12 },
        { VK.F13, F13 },
        { VK.F14, F14 },
        { VK.F15, F15 },
        { VK.F16, F16 },
        { VK.F17, F17 },
        { VK.F18, F18 },
        { VK.F19, F19 },
        { VK.F20, F20 },
        { VK.F21, F21 },
        { VK.F22, F22 },
        { VK.F23, F23 },
        { VK.F24, F24 },
        { VK.NUMLOCK, NumLock },
        { VK.SCROLL, ScrollLock },
        { VK.LSHIFT, LeftShift },
        { VK.RSHIFT, RightShift },
        { VK.LCONTROL, LeftControl },
        { VK.RCONTROL, RightControl },
        { VK.LMENU, LeftWindows },
        { VK.RMENU, RightWindows },
        { VK.BROWSER_BACK, BrowserBack },
        { VK.BROWSER_FORWARD, BrowserForward },
        { VK.BROWSER_REFRESH, BrowserRefresh },
        { VK.BROWSER_STOP, BrowserStop },
        { VK.BROWSER_SEARCH, BrowserSearch },
        { VK.BROWSER_FAVORITES, BrowserFavorites },
        { VK.BROWSER_HOME, BrowserHome },
        { VK.VOLUME_MUTE, VolumeMute },
        { VK.VOLUME_DOWN, VolumeDown },
        { VK.VOLUME_UP, VolumeUp },
        { VK.MEDIA_NEXT_TRACK, NextTrack },
        { VK.MEDIA_PREV_TRACK, PreviousTrack },
        { VK.MEDIA_STOP, Stop },
        { VK.MEDIA_PLAY_PAUSE, PlayPause },
        { VK.LAUNCH_MAIL, Mail },
        { VK.OEM_1, Semicolon },
        { VK.OEM_PLUS, Plus },
        { VK.OEM_COMMA, Comma },
        { VK.OEM_MINUS, Minus },
        { VK.OEM_PERIOD, Period },
        { VK.OEM_2, ForwardSlash },
        { VK.OEM_3, Backtick },
        { VK.OEM_4, SquareBracketOpen },
        { VK.OEM_5, BackSlash },
        { VK.OEM_6, SquareBracketClosed },
        { VK.OEM_7, Quote }
    };
}