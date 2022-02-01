using FoxEngineLib.Components;
using FoxEngineLib.Platform;

namespace FoxEngineLib;

public abstract partial class FoxEngine
{
    static readonly object _lock = new();

    static ulong _controlIndex;

    readonly int[] _fpsAvgBuffer = new int[8];

    readonly Thread _engineThread;

    readonly AudioComponent _engineSound;

    readonly Sprite _defaultDrawTarget;

    readonly Sprite _fontSprite;

    ulong _frameCount = 0;

    public static readonly Point MOUSE_OUTSIDE = new(-1, -1);

    public static FoxEngine Instance { get; private set; }

    public IPlatform Platform { get; }

    public string Name { get; }

    public Size Resolution { get; private set; }

    public Size OriginalResolution { get; private set; }

    public int ResolutionMultiplier;

    public int FramesPerSecond { get; private set; }

    public bool IsRunning { get; private set; }

    public bool IsHovered { get; internal set; }

    public bool IsFocused { get; internal set; } = true;

    public PixelMode PixelMode { get; set; } = PixelMode.NORMAL;

    public Sprite DrawTarget { get; private set; }

    public static Dictionary<KeyboardButton, ControlButton> Keyboard = new();

    public static Dictionary<MouseButton, ControlButton> Mouse = new();

    public static Point RealMousePosition;

    public static Point MousePosition;

    public uint AudioSampleFrequency { get; set; } = 44100;

    public AudioChannel AudioChannels { get; set; } = AudioChannel.Mono;

    public FoxEngine(int width = 640, int height = 480, int pixelMult = 2, string name = "Fox Engine")
    {
        GenerateUserInput();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Platform = new Platform.Windows.WindowsPlatform(this);
        }
        else
        {
            throw new NotImplementedException($"{RuntimeInformation.OSDescription} platform not yet supported.");
        }

        _fontSprite = GenerateFontSprite();

        Name = name;

        OriginalResolution = Resolution = new Size(width, height);

        ResolutionMultiplier = pixelMult;

        Instance = this;

        DrawTarget = _defaultDrawTarget = new Sprite(Resolution);

        _engineThread = new Thread(new ThreadStart(ThreadRun))
        {
            Name = "FoxEngine.Thread"
        };

        _engineSound = new AudioComponent();

        Resize(width * pixelMult, height * pixelMult);
    }

    public abstract void Create();

    public abstract void Update(double frameTime);

    public abstract void Destroy();

    public abstract double GenerateSample(uint channel, double time, double timeStep);

    public void Run()
    {
        if (Platform == null)
        {
            throw new ApplicationException("The platform initialization failed");
        }

        Platform.Initialize();

        _engineThread.Start();

        _engineSound.Start(AudioSampleFrequency, AudioChannel.Mono);

        Platform.Run();
    }

    public void Stop()
    {
        IsRunning = false;

        // User destroy method
        Destroy();

        _engineThread.Join();

        _engineSound.Destroy();

        Platform.Dispose();
    }

    public void Resize(int width, int height)
    {
        if (Resolution.Width == width && Resolution.Height == height)
        {
            // noop
            return;
        }

        Resolution = new Size(width, height);
    }

    void ThreadRun()
    {
        Platform.CreateGlContext();

        IsRunning = true;

        DrawString(10, 10, $"{Name} Engine 1.0\nREADY!", Pixel.Black, 2);

        Create();

        var stopWatch = Stopwatch.StartNew();

        while (IsRunning)
        {
            stopWatch.Stop();

            var elapsedTime = stopWatch.Elapsed;

            stopWatch = Stopwatch.StartNew();

            Update(elapsedTime.TotalMilliseconds);

            Platform.Draw();

            _fpsAvgBuffer[_frameCount % 8] = (int)Math.Ceiling(1.0f / elapsedTime.TotalMilliseconds * 1000);

            FramesPerSecond = Convert.ToInt32(_fpsAvgBuffer.Average());

            _frameCount++;
        }
    }

    internal static ulong GetControlIndex()
    {
        lock (_lock)
        {
            return _controlIndex++;
        }
    }
}