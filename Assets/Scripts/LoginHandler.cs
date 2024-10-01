using UnityEngine;
using Firebase.Auth;
using TMPro;
using Firebase.Extensions;
using UnityEngine.SceneManagement;

public class LoginHandler : MonoBehaviour
{
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public TMP_Text feedbackText;

    private FirebaseAuth auth;

    void Start()
    {
        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

public void LoginUser()
{
    string email = emailInputField.text.Trim();
    string password = passwordInputField.text;

    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
    {
        UpdateFeedback("Please enter email and password.");
        return;
    }

    // Start the sign in process
    auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
    {
        if (task.IsCanceled)
        {
            UpdateFeedback("Login was canceled.");
            return;
        }
        if (task.IsFaulted)
        {
            Debug.LogError("Login encountered an error: " + task.Exception);
            return;
        }

        // Firebase user has been logged in
        FirebaseUser user = task.Result.User; // Note the use of .User here
        UpdateFeedback("User logged in successfully: " + user.Email);
        SceneManager.LoadScene("Level 1");
    });
}

    private void UpdateFeedback(string message)
    {
        feedbackText.text = message;
    }
}