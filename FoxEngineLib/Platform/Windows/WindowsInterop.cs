using static FoxEngineLib.Platform.Windows.WindowsTypes;

namespace FoxEngineLib.Platform.Windows;

internal class WindowsInterop
{
    private const string DllUser32 = "user32.dll";
    private const string DllGdi32 = "gdi32.dll";
    private const string DllKernel32 = "kernel32.dll";
    private const string DllOpenGl32 = "opengl32.dll";

    /* PIXELFORMATDESCRIPTOR flags */
    internal const uint PFD_DOUBLEBUFFER = 0x00000001;
    internal const uint PFD_STEREO = 0x00000002;
    internal const uint PFD_DRAW_TO_WINDOW = 0x00000004;
    internal const uint PFD_DRAW_TO_BITMAP = 0x00000008;
    internal const uint PFD_SUPPORT_GDI = 0x00000010;
    internal const uint PFD_SUPPORT_OPENGL = 0x00000020;
    internal const uint PFD_GENERIC_FORMAT = 0x00000040;
    internal const uint PFD_NEED_PALETTE = 0x00000080;
    internal const uint PFD_NEED_SYSTEM_PALETTE = 0x00000100;
    internal const uint PFD_SWAP_EXCHANGE = 0x00000200;
    internal const uint PFD_SWAP_COPY = 0x00000400;
    internal const uint PFD_SWAP_LAYER_BUFFERS = 0x00000800;
    internal const uint PFD_GENERIC_ACCELERATED = 0x00001000;
    internal const uint PFD_SUPPORT_DIRECTDRAW = 0x00002000;

    /* PIXELFORMATDESCRIPTOR flags for use in ChoosePixelFormat only */
    internal const uint PFD_DEPTH_DONTCARE = 0x20000000;
    internal const uint PFD_DOUBLEBUFFER_DONTCARE = 0x40000000;
    internal const uint PFD_STEREO_DONTCARE = 0x80000000;

    internal const byte PFD_TYPE_RGBA = 0;
    internal const byte PFD_MAIN_PLANE = 0;

    // GL Enums
    public const uint GL_NO_ERROR = 0;
    public const uint GL_QUADS = 7;
    public const uint GL_UNSIGNED_BYTE = 5121;
    public const uint GL_RGBA = 6408;
    public const uint GL_DECAL = 8449;
    public const uint GL_TEXTURE_ENV_MODE = 8704;
    public const uint GL_TEXTURE_ENV = 8960;
    public const uint GL_NEAREST = 9728;
    public const uint GL_LINEAR = 9729;
    public const uint GL_TEXTURE_2D = 3553;
    public const uint GL_TEXTURE_MAG_FILTER = 10240;
    public const uint GL_TEXTURE_MIN_FILTER = 10241;
    public const int GL_COLOR_BUFFER_BIT = 16384;
    public const uint GL_RGBA8 = 32856;

    internal delegate IntPtr WndProc(IntPtr hWnd, WindowsMessages msg, IntPtr wParam, IntPtr lParam);

    [DllImport(DllGdi32)]
    internal static extern int ChoosePixelFormat(IntPtr hdc, [In] ref PIXELFORMATDESCRIPTOR ppfd);

    [DllImport(DllGdi32, SetLastError = true)]
    internal static extern bool SetPixelFormat(IntPtr hdc, int format, [In] ref PIXELFORMATDESCRIPTOR ppfd);

    [DllImport(DllGdi32)]
    internal static extern bool SwapBuffers(IntPtr hdc);

    [DllImport(DllKernel32, CharSet = CharSet.Unicode)]
    internal static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport(DllUser32)]
    internal static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport(DllUser32)]
    internal static extern int GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [DllImport(DllUser32)]
    internal static extern bool TranslateMessage([In] ref MSG lpMsg);

    [DllImport(DllUser32)]
    internal static extern IntPtr DispatchMessage([In] ref MSG lpMsg);

    [DllImport(DllUser32, SetLastError = true, CharSet = CharSet.Auto)]
    internal static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport(DllUser32, CharSet = CharSet.Unicode)]
    internal static extern int MessageBox(IntPtr hWnd, string text, string caption, MessageBoxOptions options);

    [DllImport(DllUser32)]
    [return: MarshalAs(UnmanagedType.U2)]
    internal static extern short RegisterClassEx([In] ref WNDCLASSEX lpwcx);

    [DllImport(DllUser32, SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern IntPtr CreateWindowEx(WindowStylesEx dwExStyle, string lpClassName, string lpWindowName, WindowStyles dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

    [DllImport(DllUser32, CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool DestroyWindow(IntPtr hwnd);

    [DllImport(DllUser32)]
    internal static extern void PostQuitMessage(int nExitCode);

    [DllImport(DllUser32)]
    internal static extern IntPtr DefWindowProc([In] IntPtr hWnd, [In] WindowsMessages uMsg, [In] IntPtr wParam, [In] IntPtr lParam);

    [DllImport(DllUser32)]
    internal static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommands nCmdShow);

    [DllImport(DllUser32)]
    internal static extern bool UpdateWindow(IntPtr hWnd);

    [DllImport(DllUser32, SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern bool SetWindowTextW(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)] string lpString);

    [DllImport(DllUser32, SetLastError = true)]
    internal static extern bool AdjustWindowRect(ref RECT lpRect, WindowStyles dwStyle, bool bMenu);

    [DllImport(DllUser32)]
    internal static extern bool AdjustWindowRectEx(ref RECT lpRect, WindowStyles dwStyle, bool bMenu, WindowStylesEx dwExStyle);

    [DllImport(DllUser32, CharSet = CharSet.Unicode)]
    internal static extern IntPtr LoadIcon(IntPtr hInstance, string lpIconName);

    [DllImport(DllUser32)]
    internal static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIconName);

    [DllImport(DllUser32)]
    internal static extern IntPtr LoadIcon(IntPtr hInstance, SystemIcons lpIconName);

    [DllImport(DllUser32)]
    internal static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

    [DllImport(DllUser32)]
    internal static extern IntPtr LoadCursor(IntPtr hInstance, SystemCursors lpCursorName);
    
    [DllImport(DllUser32)]
    internal static extern bool TrackMouseEvent(ref TRACKMOUSEEVENT lpEventTrack);

    [DllImport(DllOpenGl32)]
    internal static extern IntPtr wglCreateContext(IntPtr hDc);

    [DllImport(DllOpenGl32)]
    internal static extern IntPtr wglDeleteContext(IntPtr hDc);

    [DllImport(DllOpenGl32)]
    internal static extern bool wglMakeCurrent(IntPtr hDc, IntPtr newContext);

    [DllImport(DllOpenGl32, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
    internal static extern IntPtr wglGetProcAddress(string functionName);

    internal delegate bool wglSwapInterval(int interval);

    [DllImport(DllOpenGl32)]
    internal static extern void glViewport(GLint x, GLint y, GLsizei width, GLsizei height);

    [DllImport(DllOpenGl32)]
    internal static extern void glClear(GLbitfield mask);

    [DllImport(DllOpenGl32)]
    internal static extern void glEnable(GLenum cap);

    [DllImport(DllOpenGl32)]
    internal static extern void glGenTextures(GLsizei n, ref GLuint textures);

    [DllImport(DllOpenGl32)]
    internal static extern void glBindTexture(GLenum target, GLuint texture);

    [DllImport(DllOpenGl32)]
    internal static extern void glTexParameterf(GLenum target, GLenum pname, GLfloat param);

    [DllImport(DllOpenGl32)]
    internal static extern void glTexParameteri(GLenum target, GLenum pname, GLint param);

    [DllImport(DllOpenGl32)]
    internal static extern void glTexEnvf(GLenum target, GLenum pname, GLfloat param);

    [DllImport(DllOpenGl32)]
    internal static extern void glTexEnvi(GLenum target, GLenum pname, GLint param);

    [DllImport(DllOpenGl32)]
    internal static extern void glTexImage2D(GLenum target, GLint level, GLint internalFormat, GLsizei width, GLsizei height, GLint border, GLenum format, GLenum type, IntPtr data);

    [DllImport(DllOpenGl32)]
    internal static extern void glTexSubImage2D(GLenum target, GLint level, GLint xoffset, GLint yoffset, GLsizei width, GLsizei height, GLenum format, GLenum type, IntPtr data);

    [DllImport(DllOpenGl32)]
    internal static extern void glBegin(GLenum mode);

    [DllImport(DllOpenGl32)]
    internal static extern void glEnd();

    [DllImport(DllOpenGl32)]
    internal static extern GLenum glGetError();

    [DllImport(DllOpenGl32)]
    internal static extern void glTexCoord2f(GLfloat s, GLfloat t);

    [DllImport(DllOpenGl32)]
    internal static extern void glVertex3f(GLfloat x, GLfloat y, GLfloat z);
}