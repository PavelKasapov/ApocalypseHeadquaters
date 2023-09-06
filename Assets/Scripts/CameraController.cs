using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float cameraSpeed = 50f;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask clickableLayerMask;

    private Vector2 cameraMovementDirection;
    private Coroutine cameraMovementCoroutine;

    public Camera MainCamera => mainCamera;

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
            cameraTransform.position += cameraSpeed * Time.deltaTime * (Vector3)cameraMovementDirection;
            yield return null;
        }

        cameraMovementCoroutine = null;
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

    public Vector2 ScreenToWorldPoint(Vector2 cursorPosition) => mainCamera.ScreenToWorldPoint(cursorPosition);

    /*private void Update()
    {
        Debug.Log( EventSystem.current.IsPointerOverGameObject());
    }*/
}
