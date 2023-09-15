using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class MovementSystem : MonoBehaviour
{
    [Inject] private SightSystem sightSystem;
    [Inject(Id = "MainTransform")] private Transform mainTransform;
    [Inject(Id = "ModelTransform")] private Transform modelTransform;

    [SerializeField] private float speed = 3f;
    [SerializeField] private float rotationalSpeed = 240f;
    [SerializeField] private LineRenderer pathRenderer;

    private Coroutine moveCoroutine;
    private Coroutine rotateCoroutine;
    private Vector3 pointToMove;
    private float moveAngle;
    private float speedAngleModifier;

    private double distanceCheckValue = Vector3.one.sqrMagnitude * 0.01;

    private void Awake()
    {
        sightSystem.OnTargetChange += () => rotateCoroutine ??= StartCoroutine(RotateRoutine());
    }

    private void OnDestroy()
    {
        sightSystem.OnTargetChange -= () => rotateCoroutine ??= StartCoroutine(RotateRoutine());
    }

    public void MoveCharacter(Vector3 point)
    {
        var moveDirection = point - modelTransform.position;
        moveAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        pointToMove = point;
        if (moveCoroutine == null)
        {
            moveCoroutine = StartCoroutine(MoveRoutine());
        }
        if (rotateCoroutine == null)
        {
            rotateCoroutine = StartCoroutine(RotateRoutine());
        }
    }

    IEnumerator MoveRoutine()
    {
        pathRenderer.enabled = true; 
        while ((mainTransform.position - pointToMove).sqrMagnitude > distanceCheckValue)
        {
            pathRenderer.SetPosition(0, mainTransform.position);
            pathRenderer.SetPosition(1, pointToMove);

            mainTransform.Translate((pointToMove - mainTransform.position).normalized * speed * speedAngleModifier * Time.deltaTime);

            yield return null;
        }
        pathRenderer.enabled = false;
        moveCoroutine = null;
    }

    IEnumerator RotateRoutine()
    {
        float angleBetween = float.MaxValue;

        while (sightSystem.MainTarget != null || (moveCoroutine != null && angleBetween > 0.01f))
        {
            Quaternion lookRotation;
            if (sightSystem.MainTarget != null)
            {
                var target = sightSystem.MainTarget.Transform.position - mainTransform.position;
                var targetAngle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
                lookRotation = Quaternion.AngleAxis(targetAngle - 90, Vector3.forward);
            }
            else
            {
                lookRotation = Quaternion.AngleAxis(moveAngle - 90, Vector3.forward);
            }

            var rotationAngle = modelTransform.localEulerAngles.z + 90;

            angleBetween = Math.Abs((rotationAngle - moveAngle) % 360);
            angleBetween = angleBetween > 180f ? 360 - angleBetween : angleBetween;

            speedAngleModifier = (180 - angleBetween) / 180f * 0.5f + 0.5f;

            modelTransform.rotation = Quaternion.RotateTowards(modelTransform.rotation, lookRotation, Time.deltaTime * rotationalSpeed);
            
            yield return null;
        }

        speedAngleModifier = 1f;
        rotateCoroutine = null;
    }
}
