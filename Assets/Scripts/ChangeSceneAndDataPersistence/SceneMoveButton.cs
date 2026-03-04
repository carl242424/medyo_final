using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneMoveButton : MonoBehaviour
{
    // Check the scene build index and move to that specific Scene
    public int sceneBuildIndex;
    private Button button;
    private Text buttonText;

    // Start is called before the first frame update
    private void Start()
    {
        button = GetComponent<Button>();
        if (button == null) return;
        buttonText = GetComponentInChildren<Text>(); // Optional; may be null if using TextMeshPro
    }

    /// <summary>Called by UI Button onClick. Works with Standalone Input Module (touch on Android).</summary>
    public void LoadLevel()
    {
        if (sceneBuildIndex < 0) return;
        DontDestroy.DestroyPersistingObjects();
        SceneManager.LoadScene(sceneBuildIndex, LoadSceneMode.Single);
    }
}
