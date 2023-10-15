using FoxEngineLib.Components;
using NAudio.Wave;

namespace FoxEngineLib;

public abstract partial class FoxEngine
{
    public WaveOutEvent AudioOutEvent { get; private set; }

    public VUMeterMixingSampleProvider MainMixer { get; private set; }

    void InitializeAudioSystems()
    {
        MainMixer = new(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2))
        {
            ReadFully = true
        };

        AudioOutEvent = new();

        AudioOutEvent.Init(MainMixer);

        AudioOutEvent.Play();
    }
}
