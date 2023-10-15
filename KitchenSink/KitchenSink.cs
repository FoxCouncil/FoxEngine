using FoxEngineLib.Types.UI;

internal partial class KitchenSink : FoxEngine
{
    GameState _gameState;

    readonly List<IDrawable> _interface = new();

    public override void Create()
    {
        var mainMenuButton = new Button(-1, -1, 140, 20, "Kitchen Sink");
        mainMenuButton.OnClick += (s, e) => _gameState = GameState.MainMenu;
        _interface.Add(mainMenuButton);

        var controllerTest = new Button(139, -1, 140, 20, "Input Tests");
        controllerTest.OnClick += (s, e) => _gameState = GameState.Control;
        _interface.Add(controllerTest);

        var soundTest = new Button(279, -1, 140, 20, "Sound Tests");
        soundTest.OnClick += (s, e) => _gameState = GameState.Sound;
        _interface.Add(soundTest);


        //CreateMainMenu();
        //CreateControlTest();
        CreateSoundTest();

        _gameState = GameState.MainMenu;
    }

    public override void Destroy()
    {
        // throw new NotImplementedException();
    }

    public override void Update(double frameTime)
    {
        Clear(Pixel.Blue);
        
        DrawRectFilled(0, 0, OriginalResolution.Width, 20, Pixel.White);
        DrawRect(0, -1, OriginalResolution.Width, 20, Pixel.Black);

        DrawString(OriginalResolution.Width - 60, 6, $"{FramesPerSecond} FPS", Pixel.Black);

        DrawString(OriginalResolution.Width - 110, 6, "FOCUS", IsFocused ? Pixel.Black : Pixel.Red);
        DrawString(OriginalResolution.Width - 160, 6, "HOVER", IsHovered ? Pixel.Black : Pixel.Red);

        foreach (var control in _interface)
        {
            control.Draw(this);
        }

        switch (_gameState)
        {
            case GameState.MainMenu: DrawMainMenu(); break;
            case GameState.Control: DrawControlTest(); break;
            case GameState.Sound: DrawSoundTest(); break;
        }
    }
}

public enum GameState
{
    MainMenu,
    Control,
    Sound
}