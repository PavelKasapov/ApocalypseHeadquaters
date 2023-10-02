using System.Collections;
using UnityEngine;
using Zenject;

public class TargetAimDrawer : MonoBehaviour
{
    [SerializeField] LineRenderer targetAimLine;

    [Inject(Id = "MainTransform")] new readonly Transform transform;

    Coroutine drawAimCoroutine;
    private Target lockTarget;

    public void SetTarget(Target lockTarget)
    {
        this.lockTarget = lockTarget;
        targetAimLine.enabled = lockTarget != null;

        if (lockTarget != null && drawAimCoroutine == null)
        {
            drawAimCoroutine = StartCoroutine(DrawAimRoutine());
        }
    }

    IEnumerator DrawAimRoutine()
    {
        while (lockTarget != null)
        {
            var selfPosition = transform.position;
            var targetPosition = lockTarget.targetInfo.Transform.position;

            targetAimLine.SetPosition(0, selfPosition);
            targetAimLine.SetPosition(1, targetPosition);

            yield return null;
        }
        drawAimCoroutine = null;
    }
}
