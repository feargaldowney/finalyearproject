using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking; // For UnityWebRequest


public class ItemCollector : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public MicrophoneInput microphoneInput;
    public WordDatasetManager wordDatasetManager;


    private int cherries = 0;
    [SerializeField] private TMP_Text cherriesText;
    [SerializeField] private TMP_Text RandomPhrase;
    [SerializeField] private TMP_Text accuracyDisplay;
    private List<string> prompts = new List<string>();
    private List<string> userResponses = new List<string>();

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

        float waitTime = 3.5f;
        bool inputDetected = false;

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
            StartCoroutine(SendAudioToGoogleAPI(audioBase64, RandomPhrase.text)); // Pass the current prompt as a parameter
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
        playerMovement.Jump();
    }

    private void DisplayRandomWord()
    {
        string chosenWord = wordDatasetManager.GetRandomWord();
        RandomPhrase.text = chosenWord;
        prompts.Add(chosenWord); // Store the prompt
    }

    private IEnumerator SendAudioToGoogleAPI(string audioBase64, string currentPrompt)
    {
        string apiKey = "AIzaSyC8FhwyCNI87kI4wZPt8RHshn-V4vKaSYs";
        string url = $"https://speech.googleapis.com/v1/speech:recognize?key={apiKey}";

        string jsonPayload = $"{{\"config\": {{\"encoding\":\"LINEAR16\", \"sampleRateHertz\":44100, \"languageCode\":\"en-US\", \"speechContexts\": [{{\"phrases\": [\"{currentPrompt}\"]}}]}}, \"audio\": {{\"content\":\"{audioBase64}\"}}}}";
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(jsonPayload);

        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(url, "POST"))
        {
            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("Google Speech-to-Text Response: " + www.downloadHandler.text);
                ProcessResponse(www.downloadHandler.text); 
            }
        }
    }

    private void ProcessResponse(string responseJson)
    {
        // Unity's JsonUtility to parse the response
        GoogleSpeechToTextResponse response = JsonUtility.FromJson<GoogleSpeechToTextResponse>(responseJson);

        // Assuming the response contains at least one result and one alternative
        if (response.results != null && response.results.Length > 0 && response.results[0].alternatives != null && response.results[0].alternatives.Length > 0)
        {
            string transcription = response.results[0].alternatives[0].transcript;
            Debug.Log("Transcribed text: " + transcription);
            userResponses.Add(transcription); // Store the user's response
        }
        else
        {
            Debug.LogError("No transcription found in the response.");
            userResponses.Add(""); // Adding an empty string to maintain the count
        }
    }

    public float Accuracy { get; private set; }  // This property stores the accuracy value

    private List<string> incorrectWords = new List<string>();

    public void AnalyzeUserResponses()
    {
        int totalWords = 0;
        int correctWords = 0;

        for (int i = 0; i < prompts.Count; i++)
        {
            string[] promptWords = prompts[i].Split(' ');
            string[] responseWords = userResponses[i].Split(' ');

            totalWords += promptWords.Length;

            for (int j = 0; j < promptWords.Length; j++)
            {
                if (j < responseWords.Length && string.Equals(promptWords[j], responseWords[j], StringComparison.OrdinalIgnoreCase))
                {
                    correctWords++;
                }
                else
                {
                    incorrectWords.Add(promptWords[j]); // Add incorrect word to the list
                }
            }
        }

        Accuracy = totalWords > 0 ? (float)correctWords / totalWords * 100 : 0;
        Debug.Log($"User's speech word-by-word accuracy: {Accuracy}%");
        
        if (accuracyDisplay != null)
        {
            accuracyDisplay.text = $"Accuracy: {Accuracy:F2}%";
        }
    }
    
    public List<string> GetIncorrectWords()
    {
        return new List<string>(incorrectWords);  // Return a copy of the list to prevent external modifications
    }


    [Serializable]
    public class GoogleSpeechToTextResponse
    {
        public Result[] results;
    }

    [Serializable]
    public class Result
    {
        public Alternative[] alternatives;
    }

    [Serializable]
    public class Alternative
    {
        public string transcript;
    }
}
