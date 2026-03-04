using UnityEngine;
using UnityEngine.SceneManagement; // Required for switching scenes

public class LevelLoader : MonoBehaviour
{
    // This function will be called by your UI Buttons
    public void OpenLevel(string levelName)
    {
        // Loads the scene by the name you type in the Inspector
        SceneManager.LoadScene(levelName);
    }

    // Optional: A back button function to return to the Title Screen
    public void BackToMenu()
    {
        SceneManager.LoadScene("Title Screen");
    }
}