using UnityEngine;

public class MicrophoneInput : MonoBehaviour
{
    public AudioSource audioSource;
    private AudioClip recording;
    private bool isRecording = false;
    private const float threshold = 0.000002f;

    public void StartRecording(int duration = 3)
    {
        if (!isRecording)
        {
            Debug.Log("Recording started.");
            recording = Microphone.Start(null, false, duration, 44100);
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
            audioSource.clip = recording;
        }
    }

    public bool MicrophoneInputDetected()
    {
        if (recording == null || !isRecording) return false;

        float[] samples = new float[128];
        int microphonePosition = Microphone.GetPosition(null) - (samples.Length + 1);
        if (microphonePosition < 0) return false;

        recording.GetData(samples, microphonePosition);

        foreach (var sample in samples)
        {
            if (Mathf.Abs(sample) > threshold)
                return true;
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

        return WavUtility.ConvertAudioClipToBase64(recording);
    }
}
