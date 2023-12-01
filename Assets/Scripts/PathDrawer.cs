using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class PathDrawer : MonoBehaviour
{
    [SerializeField] private LineRenderer pathRenderer;

    [Inject(Id = "MainTransform")] private new readonly Transform transform;
    [Inject] private readonly MovementSystem movementSystem;

    private Coroutine updatePathCoroutine;
    private Queue<Vector3> path;

    private void Awake()
    {
        movementSystem.OnPathGenerated += DrawPath;
    }

    public void DrawPath(Queue<Vector3> path)
    {
        this.path = path;
        pathRenderer.positionCount = path.Count + 1;
        pathRenderer.SetPositions(path.Reverse().ToArray());
        pathRenderer.SetPosition(path.Count, transform.position);

        if (updatePathCoroutine == null) updatePathCoroutine = StartCoroutine(UpdatePathRoutine());
    }

    IEnumerator UpdatePathRoutine()
    {
        pathRenderer.enabled = true;
        while (path.Count > 0)
        {
            if (pathRenderer.positionCount != path.Count + 1)
            {
                pathRenderer.positionCount = path.Count + 1;
            }
            pathRenderer.SetPosition(path.Count, transform.position);
            yield return null;
        }
        updatePathCoroutine = null;
    }
}