using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using Firebase.Extensions;


public class RegisterHandler : MonoBehaviour
{
    public TMPro.TMP_InputField emailInputField;
    public TMPro.TMP_InputField passwordInputField;
    public TMPro.TMP_InputField confirmPasswordInputField;
    public TMPro.TMP_Text feedbackText; // Display registration errors or messages here

    private FirebaseAuth auth;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogError($"Failed to initialize Firebase with {task.Exception}");
                return;
            }

            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Initialize the Firebase auth variable here after confirming that Firebase is available.
                auth = FirebaseAuth.DefaultInstance;
                Debug.Log("Firebase Auth has been successfully initialized.");
            }
            else
            {
                Debug.Log($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });
    }


    public void RegisterUser()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;
        string confirmPassword = confirmPasswordInputField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            feedbackText.text = "Please enter all fields.";
            return;
        }

        if (password != confirmPassword)
        {
            feedbackText.text = "Passwords do not match.";
            return;
        }

        Debug.Log(" Creating User...");
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            Debug.Log(" Creating User within function");

            if (task.IsCanceled)
            {
                UpdateFeedback("Registration was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                // UpdateFeedback("Registration encountered an error: " + task.Exception);
                // was originially showing errors on screen but changed this to debug so that specific errors would not
                // be shown to users as they could be used to exploit the app or may confuse the user.
                Debug.LogError("Registration encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created
            Debug.Log(" Finishing creation of User...");
            FirebaseUser newUser = task.Result.User; 
            UpdateFeedback("User registered successfully: " + newUser.Email);
            // part of future work if I were to continue development would be to include auto login feature for users with accounts.
            Debug.Log("User created!");

            SceneManager.LoadScene("Level 1");
        });
    }

    void UpdateFeedback(string message)
    {
        feedbackText.text = message;
    }
}
