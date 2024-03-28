using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLevel : MonoBehaviour
{
    private AudioSource finishSoundEffect;
    public ItemCollector ItemCollector;
    public bool levelCompleted = false;
    void Start()
    {
        finishSoundEffect = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player" && !levelCompleted)
        {
            finishSoundEffect.Play();
            levelCompleted = true;
            ItemCollector.AnalyzeUserResponses();
            Invoke("CompleteLevel", 15.25f); // adds a delay to method being called. method must be in string format.
            // CompleteLevel();
        }
    }

    private void CompleteLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
