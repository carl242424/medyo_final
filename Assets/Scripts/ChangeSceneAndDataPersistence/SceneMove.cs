using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // <--- YOU NEED THIS TO USE 'IMAGE'

public class SceneMove : MonoBehaviour
{
    public int sceneBuildIndex;
    public Vector2 spawnPosition; 
    
    // NEW: This creates the slot in the Inspector
    public Image fadeImage; 
    public float fadeSpeed = 1f;

    private bool hasTriggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered) return;

        if (collision.CompareTag("Player"))
        {
            hasTriggered = true;
            // We change this to a sequence so the fade happens first
            StartCoroutine(TransitionSequence(collision.transform));
        }
    }

    IEnumerator TransitionSequence(Transform playerTransform)
    {
        // 1. Fade to Black
        yield return StartCoroutine(Fade(1));

        // 2. Teleport the player while the screen is black
        playerTransform.position = new Vector3(spawnPosition.x, spawnPosition.y, playerTransform.position.z);
        
        // 3. Load the scene
        SceneManager.LoadScene(sceneBuildIndex, LoadSceneMode.Single);
    }

    // Helper function to handle the fading math
    IEnumerator Fade(float targetAlpha)
    {
        float alpha = fadeImage.color.a;
        while (!Mathf.Approximately(alpha, targetAlpha))
        {
            alpha = Mathf.MoveTowards(alpha, targetAlpha, fadeSpeed * Time.deltaTime);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }
}