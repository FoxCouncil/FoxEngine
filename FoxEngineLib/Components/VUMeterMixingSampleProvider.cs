using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace FoxEngineLib.Components;

public class VUMeterMixingSampleProvider : MixingSampleProvider, ISampleProvider
{
    public float LastVUValue { get; private set; }

    public VUMeterMixingSampleProvider(WaveFormat waveFormat) : base(waveFormat) { }

    public int Read(float[] buffer, int offset, int count)
    {
        int samplesRead = base.Read(buffer, offset, count);

        // Calculate VU value from buffer
        LastVUValue = CalculateVU(buffer, offset, samplesRead);

        return samplesRead;
    }

    private static float CalculateVU(float[] buffer, int offset, int samplesRead)
    {
        // Placeholder RMS calculation
        float sum = 0;

        for (int i = offset; i < offset + samplesRead; i++)
        {
            sum += buffer[i] * buffer[i];
        }

        return (float)Math.Sqrt(sum / samplesRead);
    }
}

