using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void LoadRegisterMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadLoginMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }

    public void CancelFromLogin()
    {
        SceneManager.LoadScene("Start Screen");
    }

    public void CancelFromRegister()
    {
        SceneManager.LoadScene("Start Screen");
    }

}
