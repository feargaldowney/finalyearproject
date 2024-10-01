using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth;  
using System.Collections.Generic;


public class FinishLevel : MonoBehaviour
{
    public ItemCollector itemCollector;
    private AudioSource finishSoundEffect;
    public bool levelCompleted = false;

    private void Start()
    {
        finishSoundEffect = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player" && !levelCompleted)
        {
            finishSoundEffect.Play();
            levelCompleted = true;
            itemCollector.AnalyzeUserResponses();
            CompleteLevel();
        }
    }

    private void CompleteLevel()
    {
        int currentLevel = SceneManager.GetActiveScene().buildIndex; 
        var user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null)
        {
            string email = user.Email;
            string subject = "Your Speech Accuracy Grade";
            string body = GetAccuracyMessage(itemCollector.Accuracy, currentLevel);
            List<string> incorrectWords = itemCollector.GetIncorrectWords();

            if (SendSimpleMessage.Instance != null)
            {
                StartCoroutine(SendSimpleMessage.Instance.SendSimpleMessageEmail(email, subject, body, incorrectWords));
            }
        }

        Invoke("LoadNextScene", 2f);
    }



    private void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private Dictionary<int, string> phoneticChallenges = new Dictionary<int, string>()
    {
        {3, "TH sounds"},
        {4, "S sounds"},
        {5, "L sounds"},
        {6, "NG sounds"},
        {7, "R sounds"}
    };


    private string GetAccuracyMessage(float accuracy, int currentLevel)
    {
        string phoneticChallenge = phoneticChallenges.ContainsKey(currentLevel) ? phoneticChallenges[currentLevel] : "specific phonetic challenge";
        string reportMessage = $"This level focused on {phoneticChallenge}. ";

        if (accuracy >= 70)
        {
            reportMessage += "Your accuracy is above 70%. You likely do not require speech and language intervention.";
        }
        else if (accuracy >= 40)
        {
            reportMessage += "Your accuracy is between 40% and 70%. Monitor your progress and consider intervention if no improvement is seen.";
        }
        else
        {
            reportMessage += "Your accuracy is below 40%. It is recommended that you consider speech and language intervention.";
        }

        return reportMessage;
    }

}

