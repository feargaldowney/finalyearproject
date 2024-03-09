using UnityEngine;
using System;

public class MicrophoneInput : MonoBehaviour
{
    public AudioSource audioSource;
    private AudioClip recording;
    private bool isRecording = false;
    private const float threshold = 0.0002f; // Threshold for audio detection

    public void StartRecording(int duration = 3)
    {
        if (!isRecording)
        {
            Debug.Log("Recording started.");
            recording = Microphone.Start(null, false, duration, 44100); // params: default mic, do not loop and overwrite recording, length, Hertz
            isRecording = true;
        }
    }

    public void StopRecording()
    {
        if (isRecording)
        {
            Microphone.End(null);
            isRecording = false;
            Debug.Log("Recording stopped.");
             // Assign the recorded AudioClip to the AudioSource and play it
            audioSource.clip = recording;
            audioSource.Play();
        }
    }

    public bool MicrophoneInputDetected()
    {
        if (recording == null || !isRecording) return false;

        float[] samples = new float[128];
        int microphonePosition = Microphone.GetPosition(null) - (samples.Length + 1);
        if (microphonePosition < 0) return false;

        recording.GetData(samples, microphonePosition);

        foreach (float sample in samples)
        {
            if (Mathf.Abs(sample) > threshold)
            {
                return true;
            }
        }

        return false;
    }

    public string GetRecordedAudioAsBase64()
    {
        if (recording == null)
        {
            Debug.LogError("No recording available to convert to Base64.");
            return string.Empty;
        }

        var samples = new float[recording.samples * recording.channels];
        recording.GetData(samples, 0);

        // Assuming the audio is mono and 16-bit PCM, each sample is a float that needs to be converted to a short.
        var samplesShort = new short[samples.Length];
        for (int i = 0; i < samples.Length; i++)
        {
            samplesShort[i] = (short)(samples[i] * short.MaxValue);  // Convert float [-1.0, 1.0] to short [-32768, 32767]
        }

        var byteArray = new byte[samplesShort.Length * sizeof(short)];
        Buffer.BlockCopy(samplesShort, 0, byteArray, 0, byteArray.Length);
        
        return Convert.ToBase64String(byteArray);
    }

}
