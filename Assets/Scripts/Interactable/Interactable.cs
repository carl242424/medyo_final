using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Interactable : MonoBehaviour
{
    public bool isInRange;
    public UnityEvent interactAction;
    public GameObject pressE;

    private PlayerInput playerInput;

    private void Start()
    {
        pressE.SetActive(false);
        StartCoroutine(SubscribeToPlayerInputWhenReady());
    }

    private IEnumerator SubscribeToPlayerInputWhenReady()
    {
        for (int i = 0; i < 30; i++)
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerInput = player.GetComponent<PlayerInput>();
                if (playerInput != null)
                {
                    playerInput.actions["Player/Interact"].started += OnInteract;
                    yield break;
                }
            }
            yield return null;
        }
    }

    private void OnDestroy()
    {
        if (playerInput != null)
        {
            playerInput.actions["Player/Interact"].started -= OnInteract;
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (isInRange)
        {
            interactAction.Invoke();
        }
        else
        {
            Debug.Log("Not in range of any interactable objects.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isInRange = true;
            pressE.SetActive(true);
            Debug.Log("Player is now in range.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isInRange = false;
            pressE.SetActive(false);
            Debug.Log("Player is no longer in range.");
        }
    }
}
