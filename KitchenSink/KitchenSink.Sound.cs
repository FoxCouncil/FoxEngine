using FoxEngineLib.Types.UI;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Threading.Channels;

internal partial class KitchenSink : FoxEngine
{
    Button deleteAll = new(5, 20, 25, 20, "D");

    Button addSoundGenerator = new(35, 20, 60, 20, "Pink");

    Button addSoundGenerator2 = new(100, 20, 60, 20, "White");

    Button addSoundGenerator3 = new(165, 20, 60, 20, "MP3");

    void CreateSoundTest()
    {
        deleteAll.OnClick += (s, e) => MainMixer.RemoveAllMixerInputs();

        addSoundGenerator.OnClick += (s, e) => MainMixer.AddMixerInput(new SignalGenerator()
        {
            Type = SignalGeneratorType.Pink
        });

        addSoundGenerator2.OnClick += (s, e) => MainMixer.AddMixerInput(new SignalGenerator()
        {
            Type = SignalGeneratorType.White
        });

        addSoundGenerator3.OnClick += (s, e) => MainMixer.AddMixerInput(new MediaFoundationReader("https://media.foxcouncil.com/radio/awoo.mp3"));
    }

    void DrawSoundTest()
    {
        deleteAll.Draw(this);
        addSoundGenerator.Draw(this);
        addSoundGenerator2.Draw(this);
        addSoundGenerator3.Draw(this);

        const int meterWidth = 20;
        const int meterHeight = 300;
        const int spacing = 10;

        var value = MainMixer.LastVUValue;
        value = (float)Math.Pow(value, .42f);

        var x = 250 + (meterWidth + spacing);
        var y = 350;
        var width = meterWidth;
        var height = (int)(value * meterHeight);
        var color = GetColor(value);

        DrawRectFilled(x, y - height, width, height, color);

        var mixerProviders = MainMixer.MixerInputs;

        var y2 = 50;

        foreach (var provider in mixerProviders)
        {
            if (provider is SignalGenerator)
            {
                var sigProvider = (SignalGenerator)provider;

                DrawString(5, y2, $"GEN {sigProvider.Type}", Pixel.White);
            }
            else
            {
                DrawString(5, y2, provider.GetType().Name, Pixel.White);
            }

            y2 += 15;
        }
    }

    private Pixel GetColor(float value)
    {
        if (value > 0.8f) return Pixel.Red;
        if (value > 0.5f) return Pixel.Yellow;
        return Pixel.Green;
    }
}