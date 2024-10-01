using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;


public class SendSimpleMessage : MonoBehaviour
{
    public static SendSimpleMessage Instance;
    
    // Mailgun settings
    public const string MailgunDomain = "sandbox49088e483ab741918f280a97b2fbf4ba.mailgun.org";
    public const string MailgunApiKey = "83098a0a2379e5d2ebbc66e02fa12148-2175ccc2-373e4ac3"; 
    public const string FromEmail = "feargaldowney123@gmail.com";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator SendSimpleMessageEmail(string recipientEmail, string subject, string body, List<string> incorrectWords)
    {
        string incorrectWordsText = incorrectWords.Count > 0 ? "\nWords to improve: " + string.Join(", ", incorrectWords) : "";
        string fullMessage = body + incorrectWordsText;

        string url = $"https://api.mailgun.net/v3/{MailgunDomain}/messages";
        string auth = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"api:{MailgunApiKey}"));

        WWWForm form = new WWWForm();
        form.AddField("from", FromEmail);
        form.AddField("to", recipientEmail);
        form.AddField("subject", subject);
        form.AddField("text", fullMessage);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            www.SetRequestHeader("Authorization", $"Basic {auth}");
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error sending mail: {www.error}");
            }
            else
            {
                Debug.Log("Mail sent successfully to " + recipientEmail);
            }
        }
    }

}
