using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Networking; // For UnityWebRequest


public class ItemCollector : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public CredentialsLoader credentialsLoader;
    public MicrophoneInput microphoneInput;

    private int cherries = 0;
    [SerializeField] private TMP_Text cherriesText;
    [SerializeField] private TMP_Text RandomPhrase;
    private string[] words = { "cat", "hat", "pat", "tip", "hello", "pot", "My name is", "The big house" };

    [SerializeField] private AudioSource collectSoundEffect;

    public static bool isPaused = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Cherry"))
        {
            Destroy(collision.gameObject);
            cherries++;
            collectSoundEffect.Play();
            cherriesText.text = "Cherries: " + cherries;
        }
        else if (collision.gameObject.CompareTag("Pineapple"))
        {
            Destroy(collision.gameObject);
            collectSoundEffect.Play();
            PauseGameAndStartRecording();
        }
    }

    private void PauseGameAndStartRecording()
    {
        Time.timeScale = 0f;
        isPaused = true;
        DisplayRandomWord();
        StartCoroutine(HandleRecordingAndResume());
    }

    private IEnumerator HandleRecordingAndResume()
    {
        microphoneInput.StartRecording(3); // Specify 3 seconds recording time

        float waitTime = 3f; // Set the total wait time to 3 seconds
        bool inputDetected = false;

        // Loop for 3 seconds to continuously check for microphone input
        for (float timer = waitTime; timer > 0; timer -= Time.unscaledDeltaTime)
        {
            if (microphoneInput.MicrophoneInputDetected())
            {
                inputDetected = true;
                Debug.Log("Audio input detected");
                break; // Exit the loop early if audio input is detected
            }
            yield return null;
        }

        // If input was detected, wait for the remainder of the 3 seconds
        if (inputDetected)
        {
            yield return new WaitForSecondsRealtime(waitTime);
        }

        microphoneInput.StopRecording();
        Debug.Log("Recording stopped.");
        string audioBase64 = microphoneInput.GetRecordedAudioAsBase64();

        if (!string.IsNullOrEmpty(audioBase64))
        {
            Debug.Log("Audio data converted to Base64.");
            // Now you can use the Base64-encoded audio data to send to Google's Speech-to-Text API
            StartCoroutine(SendAudioToGoogleAPI(audioBase64));
        }
        else
        {
            Debug.LogError("Failed to convert audio to Base64.");
        }
        ResumeGameWithBonusJump();
    }

    private void ResumeGameWithBonusJump()
    {
        Time.timeScale = 1f;
        isPaused = false;
        RandomPhrase.text = ""; // Clear the displayed word/phrase
        playerMovement.EnableBonusJump();
    }

    private void DisplayRandomWord()
    {
        int index = Random.Range(0, words.Length);
        RandomPhrase.text = words[index];
    }

    private IEnumerator SendAudioToGoogleAPI(string audioBase64)
    {
        // string apiKey = credentialsLoader.GetDecryptedAPIKey();
        string url = $"https://speech.googleapis.com/v1/speech:recognize?key={"428b389fb85de980ff67cc79ae824093cac39c85"}";

        // Construct the JSON payload
        string jsonPayload = "{\"config\": {\"encoding\":\"LINEAR16\", \"sampleRateHertz\":44100, \"languageCode\":\"en-US\"}, \"audio\": {\"content\":\"" + audioBase64 + "\"}}";
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(jsonPayload);

        // Create and send the UnityWebRequest
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(url, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(postData);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("Google Speech-to-Text Response: " + www.downloadHandler.text);
                // Parse the response to extract and use the transcribed text
            }
        }
    }

}
