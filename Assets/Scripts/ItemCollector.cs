using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking; // For UnityWebRequest


public class ItemCollector : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public CredentialsLoader credentialsLoader;
    public MicrophoneInput microphoneInput;

    private int cherries = 0;
    [SerializeField] private TMP_Text cherriesText;
    [SerializeField] private TMP_Text RandomPhrase;
    [SerializeField] private TMP_Text accuracyDisplay;
    private List<string> prompts = new List<string>();
    private List<string> userResponses = new List<string>();

    private string[] words = { "cat in the hat", "rose bush", "pet the dog", "tip the can", "hello sir", "pot of gold", "My name is", "The big house", "With my pet", "The three pigs", "Seven hens", "Four ducks", "farm work", "Cows",
     "The horses run", "The quiet mouse", "The head of the bed", "garden", "Box of toys", "Jump the fence", "barn yard", "flock of sheep", "fish in the sea", "hose pipe", "shoe shop", "beans on toast", "red brick", "tea and coffee", "eggs on toast", "He lies in the hay", "It is sunny", "It is rainy", "It is cloudy", "It is cold", "It is hot",
      "grass", "The bold bull", "they smiled", "buzzing bees", "Door frame", "Tall walls", "Table top", "School yard", "Tim plays in the sun", "The lion roars", "bed time", "cheese", "frog", "tin can", "silly summer", "sit on the step", "muddy water", "red pants", "green top", "blue shoes", "carton of milk", "belly rub",
       "who, what, where?", "Give a speech", "more than", "play time", "I am sorry", " I am happy", "thank you", "you are welcome", "good bye", "good morning", "good night", "Santa wears red", "rock and roll", "blue birds", "yellow beach", "pink hair", "school bag", "birds in the trees" };

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
        microphoneInput.StartRecording(4); // Specify 4 seconds recording time

        float waitTime = 4f; // Set the total wait time to 4 seconds
        bool inputDetected = false;

        // Loop for 4 seconds to continuously check for microphone input
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

        // If input was detected, wait for the remainder of the 4 seconds
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
        int index = UnityEngine.Random.Range(0, words.Length);
        string chosenWord = words[index];
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
                ProcessResponse(www.downloadHandler.text); // You need to implement this method to parse and analyze the response.
            }
        }
    }

    private void ProcessResponse(string responseJson)
    {
        // Use Unity's JsonUtility or another JSON parsing method to parse the response
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

    public void AnalyzeUserResponses()
    {
        int totalWords = 0;
        int correctWords = 0;

        // Ensure there is a response for each prompt before proceeding
        if (userResponses.Count != prompts.Count)
        {
            Debug.LogError("The number of user responses does not match the number of prompts.");
            return;
        }

        for (int i = 0; i < prompts.Count; i++)
        {
            // Split the prompt and the user response into words
            string[] promptWords = prompts[i].Split(' ');
            string[] responseWords = userResponses[i].Split(' ');

            // Update total word count
            totalWords += promptWords.Length;

            // Compare each word in the response to the corresponding word in the prompt
            for (int j = 0; j < promptWords.Length && j < responseWords.Length; j++)
            {
                if (string.Equals(promptWords[j], responseWords[j], StringComparison.OrdinalIgnoreCase))
                {
                    correctWords++;
                }
                else
                {
                    // Log the mismatch for review
                    Debug.Log($"Mismatch in phrase '{prompts[i]}': Expected '{promptWords[j]}', but got '{responseWords[j]}'");
                }
            }
        }

        float accuracy = totalWords > 0 ? (float)correctWords / totalWords * 100 : 0;
        Debug.Log($"User's speech word-by-word accuracy: {accuracy}%");
        // Update the UI to show the accuracy
        if (accuracyDisplay != null)
        {
            accuracyDisplay.text = $"Accuracy: {accuracy:F2}%";
        }

        // Here you can decide what to do next based on the user's accuracy
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
