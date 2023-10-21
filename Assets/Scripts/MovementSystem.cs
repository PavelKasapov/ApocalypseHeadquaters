using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MovementSystem : MonoBehaviour
{
    [Inject] private CombatSystem combatSystem;
    [Inject] private PathDrawer pathDrawer;
    [Inject(Id = "MainTransform")] private Transform mainTransform;
    [Inject(Id = "ModelTransform")] private Transform modelTransform;

    [SerializeField] private float speed = 3f;
    [SerializeField] private float rotationalSpeed = 240f;

    private Coroutine moveCoroutine;
    private Coroutine rotateCoroutine;
    private Coroutine followCoroutine;
    private Vector3 pointToMove;
    private float moveAngle;
    private float speedAngleModifier;
    private Queue<Vector3> path = new Queue<Vector3>();

    private double distanceCheckValue = Vector3.one.sqrMagnitude * 0.01;

    public MovementStatus Status => (
        (moveCoroutine != null ? MovementStatus.Moving : MovementStatus.None) |
        (rotateCoroutine != null ? MovementStatus.Rotating : MovementStatus.None)
        );

    private void Awake()
    {
        combatSystem.OnTargetChange += () => rotateCoroutine ??= StartCoroutine(RotateRoutine());
    }

    private void OnDestroy()
    {
        combatSystem.OnTargetChange -= () => rotateCoroutine ??= StartCoroutine(RotateRoutine());
    }

    public void Follow(Transform targetTransform)
    {
        if (followCoroutine != null) StopCoroutine(followCoroutine);

        followCoroutine = StartCoroutine(FollowRoutine(targetTransform));
        
    }

    public void MoveToPoint(Vector3 point)
    {
        if (followCoroutine != null) StopCoroutine(followCoroutine);

        MoveCharacter(point);
    }

    IEnumerator FollowRoutine(Transform targetTransform)
    {
        while (true)
        {
            MoveCharacter(targetTransform.position);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void MoveCharacter(Vector3 point)
    {
        //Todo: Path Generation;
        path.Clear();
        path.Enqueue(point);
        //
        pathDrawer.DrawPath(path);

        if (moveCoroutine == null)
        {
            moveCoroutine = StartCoroutine(MoveRoutine());
        }
        else
        {
            pointToMove = path.Peek();
            var moveDirection = pointToMove - modelTransform.position;
            moveAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        }

        if (rotateCoroutine == null)
        {
            rotateCoroutine = StartCoroutine(RotateRoutine());
        }
    }

    IEnumerator MoveRoutine()
    {
        while (path.Count > 0)
        {
            pointToMove = path.Peek();
            var moveDirection = pointToMove - modelTransform.position;
            moveAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;

            if (rotateCoroutine == null)
            {
                rotateCoroutine = StartCoroutine(RotateRoutine());
            }

            while ((mainTransform.position - pointToMove).sqrMagnitude > distanceCheckValue)
            {
                if (!combatSystem.IsHardLock || (combatSystem.IsHardLock && !combatSystem.IsWithinDistance))
                {
                    mainTransform.Translate(speed
                    * speedAngleModifier
                    * Time.deltaTime
                    * (pointToMove - mainTransform.position).normalized);
                }
                yield return null;
            }

            path.Dequeue();
        }
        moveCoroutine = null;
    }

    IEnumerator RotateRoutine()
    {
        float angleBetween = float.MaxValue;

        while (combatSystem.LockTarget != null || (moveCoroutine != null && angleBetween > 0.01f))
        {
            Quaternion lookRotation;
            if (combatSystem.LockTarget != null)
            {
                var target = combatSystem.LockTarget.targetInfo.Transform.position - mainTransform.position;
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