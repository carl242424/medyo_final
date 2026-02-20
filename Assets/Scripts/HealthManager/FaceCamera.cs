using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private void LateUpdate()
    {
        if (Camera.main != null)
        {
            // This orientation method prevents the "mirrored text" look
            // even if the Knight flips its localScale.
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}