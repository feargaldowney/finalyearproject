using UnityEngine;
using UnityEngine.SceneManagement; // Include the SceneManager namespace
using System.Collections.Generic;

public class WordDatasetManager : MonoBehaviour
{
    private List<string> words = new List<string>();

    void Start()
    {
        // Load the dataset corresponding to the current level when the script starts
        LoadDatasetForCurrentScene();
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // This method is called whenever a new scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadDatasetForCurrentScene();
    }

    private void LoadDatasetForCurrentScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        LoadDataset("Word_datasets/WordLists/" + sceneName);
    }

    public void LoadDataset(string filePath)
    {
        TextAsset wordData = Resources.Load<TextAsset>(filePath);

        if (wordData != null)
        {
            string[] data = wordData.text.Split(new char[] { '\n' });

            words.Clear();
            foreach (string line in data)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    words.Add(line.Trim());
                }
            }
        }
        else
        {
            Debug.LogError("Failed to load dataset: " + filePath);
        }
    }

    public string GetRandomWord()
    {
        if (words.Count > 0)
        {
            int index = Random.Range(0, words.Count);
            return words[index];
        }
        else
        {
            Debug.LogError("Word list is empty.");
            return "";
        }
    }
}
