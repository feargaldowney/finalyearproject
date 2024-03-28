using System.IO;
using UnityEngine;

public static class WavUtility
{
    public static string ConvertAudioClipToBase64(AudioClip clip)
    {
        var samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        using (var memoryStream = new MemoryStream())
        {
            WriteWavHeader(memoryStream, clip.channels, clip.frequency, samples.Length);
            WriteSamples(memoryStream, samples);

            return System.Convert.ToBase64String(memoryStream.ToArray());
        }
    }

    private static void WriteWavHeader(Stream stream, int channels, int sampleRate, int sampleCount)
    {
        var fileSize = 44 + (sampleCount * 2); // WAV header size (44 bytes) + sample count * 2 bytes (16-bit samples)

        using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, true))
        {
            writer.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"));
            writer.Write(fileSize - 8); // File size - 8 bytes for 'RIFF' and file size data
            writer.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"));
            writer.Write(System.Text.Encoding.UTF8.GetBytes("fmt "));
            writer.Write(16); // Sub-chunk size (16 for PCM)
            writer.Write((short)1); // Audio format (1 for PCM)
            writer.Write((short)channels);
            writer.Write(sampleRate);
            writer.Write(sampleRate * channels * 2); // Byte rate
            writer.Write((short)(channels * 2)); // Block align
            writer.Write((short)16); // Bits per sample
            writer.Write(System.Text.Encoding.UTF8.GetBytes("data"));
            writer.Write(sampleCount * 2); // Sub-chunk 2 size (sample count * 2 bytes per sample)
        }
    }

    private static void WriteSamples(Stream stream, float[] samples)
    {
        using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, true))
        {
            foreach (var sample in samples)
            {
                var intSample = (short)(sample * short.MaxValue);
                writer.Write(intSample);
            }
        }
    }
}
