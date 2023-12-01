using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float cameraSpeed = 50f;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask clickableLayerMask;
    [SerializeField] private float zoomSpeed = 7f;

    private float targetZoom = 7f;
    private float actualZoom = 7f;
    private Vector2 cameraMovementDirection;

    private Coroutine cameraMovementCoroutine;
    private Coroutine cameraZoomCoroutine;
    public Camera MainCamera => mainCamera;

    public void MoveCamera(Vector2 direction)
    {
        cameraMovementDirection = direction;

        if (cameraMovementCoroutine == null)
        {
            cameraMovementCoroutine = StartCoroutine(CameraMovementRoutine());
        }
    }

    public void ZoomCamera(float value)
    {
        var newZoom = targetZoom - value;
        if (newZoom <= 0) return;

        targetZoom = newZoom;

        if (cameraZoomCoroutine == null)
        {
            cameraZoomCoroutine = StartCoroutine(CameraZoomRoutine());
        }
    }

    IEnumerator CameraMovementRoutine()
    {
        while (cameraMovementDirection != Vector2.zero)
        {
            cameraTransform.position += cameraSpeed * Time.deltaTime * (Vector3)cameraMovementDirection;
            yield return null;
        }

        cameraMovementCoroutine = null;
    }

    IEnumerator CameraZoomRoutine()
    {
        while (Mathf.Abs(actualZoom - targetZoom) > 0.01f)
        {
            actualZoom += Time.deltaTime * Mathf.Sign(targetZoom - actualZoom) * Mathf.Ceil(Mathf.Abs(targetZoom - actualZoom)) * zoomSpeed;
            mainCamera.orthographicSize = actualZoom;

            yield return null;
        }
        actualZoom = targetZoom;
        mainCamera.orthographicSize = actualZoom;
        
        cameraZoomCoroutine = null;
    }

    public IClickable ClickRaycast(Vector2 cursorPosition)
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

    public Vector2 ScreenToWorldPoint(Vector2 cursorPosition) => 
        mainCamera.ScreenToWorldPoint(cursorPosition);
}