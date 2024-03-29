﻿namespace FoxEngineLib.Platform;

public interface IPlatform
{
    void MessageBox(string title, string message);

    void Initialize();

    void Run();

    void Dispose();

    void CreateGlContext();

    void Draw();

    void SetWindowTitle(string title);
}
