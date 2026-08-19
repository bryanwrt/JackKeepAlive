using System;
using System.Runtime.InteropServices;
using System.Threading;
using NAudio.CoreAudioApi;
using NAudio.Wave;

internal static class Program
{
    private const float Amplitude = 1e-5f;

    private static void Main()
    {
        using var enumerator = new MMDeviceEnumerator();

        using var device = enumerator.GetDefaultAudioEndpoint(
            DataFlow.Render,
            Role.Multimedia);

        var format = WaveFormat.CreateIeeeFloatWaveFormat(
            48_000,
            2);

        using var output = new WasapiOut(
            device,
            AudioClientShareMode.Shared,
            true,
            500);

        output.Init(new KeepAliveProvider(format));

        output.Play();

        Thread.Sleep(Timeout.Infinite);
    }
}

internal sealed class KeepAliveProvider : IWaveProvider
{
    private readonly float amplitude;

    public WaveFormat WaveFormat { get; }

    public KeepAliveProvider(WaveFormat waveFormat)
    {
        WaveFormat = waveFormat;
        amplitude = Amplitude;
    }

    private const float Amplitude = 1e-5f;

    public int Read(
        byte[] buffer,
        int offset,
        int count)
    {
        var samples = MemoryMarshal.Cast<byte, float>(
            buffer.AsSpan(offset, count));

        samples.Fill(amplitude);

        return count;
    }
}