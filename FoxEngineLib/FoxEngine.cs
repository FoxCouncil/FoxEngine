﻿using FoxEngineLib.Components;
using FoxEngineLib.Platform;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace FoxEngineLib;

public abstract partial class FoxEngine
{
    public static FoxEngine Instance { get; private set; }

    public bool IsRunning { get; private set; }

    public string Name { get; }

    public IPlatform Platform { get; }

    public Size Resolution { get; private set; }

    public Size OriginalResolution { get; private set; }

    public int ResolutionMultiplier;

    public int FramesPerSecond { get; private set; }

    public ulong Frames => _frameCount;

    static readonly object _lock = new();

    ulong _frameCount = 0;

    readonly int[] _fpsAvgBuffer = new int[8];

    readonly Thread _engineThread;

    readonly Sprite _defaultDrawTarget;

    readonly Sprite _fontSprite;

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

        InitializeAudioSystems();

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

        Resize(width * pixelMult, height * pixelMult);
    }

    public abstract void Create();

    public abstract void Update(double deltaTime);

    public abstract void Destroy();

    public void Run()
    {
        if (Platform == null)
        {
            throw new ApplicationException("The platform initialization failed");
        }

        Platform.Initialize();

        _engineThread.Start();

        Platform.Run();
    }

    public void Stop()
    {
        IsRunning = false;

        // User destroy method
        Destroy();

        AudioOutEvent?.Stop();

        AudioOutEvent?.Dispose();

        _engineThread.Join();

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

            Update(elapsedTime.TotalSeconds);

            Platform.Draw();

            _fpsAvgBuffer[_frameCount % 8] = (int)Math.Ceiling(1.0f / elapsedTime.TotalMilliseconds * 1000);

            FramesPerSecond = Convert.ToInt32(_fpsAvgBuffer.Average());

            _frameCount++;
        }
    }
}