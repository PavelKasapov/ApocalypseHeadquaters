using System.Collections;
using UnityEngine;

public class PlayerControlSystem : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float cameraSpeed = 50f;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask clickableLayerMask;

    private Vector2 cameraMovementDirection;
    private Coroutine cameraMovementCoroutine;

    public void MoveCamera(Vector2 direction)
    {
        cameraMovementDirection = direction;

        if (cameraMovementCoroutine == null)
        {
            cameraMovementCoroutine = StartCoroutine(CameraMovementRoutine());
        }
    }

    IEnumerator CameraMovementRoutine()
    {
        while (cameraMovementDirection != Vector2.zero)
        {
            cameraTransform.position += (Vector3)cameraMovementDirection * cameraSpeed * Time.deltaTime;
            yield return null;
        }

        cameraMovementCoroutine = null;
    }

    public void LeftClick(Vector2 cursorPosition)
    {
        Raycast(cursorPosition)?.OnLeftClick(mainCamera.ScreenToWorldPoint(cursorPosition));
    }

    public void RightClick(Vector2 cursorPosition)
    {
        Raycast(cursorPosition)?.OnRightClick(mainCamera.ScreenToWorldPoint(cursorPosition));
    }

    private IClickable Raycast(Vector2 cursorPosition)
    {
        IClickable result = null;
        
        RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(cursorPosition), Vector2.zero, float.PositiveInfinity, clickableLayerMask);
        
        if (hit.collider != null)
        {
            Transform objectHit = hit.transform;

            objectHit.TryGetComponent<IClickable>(out result);
        }

        return result;
    }
}
