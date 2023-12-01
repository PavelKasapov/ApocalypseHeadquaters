using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class MovementSystem : MonoBehaviour
{
    [Inject(Id = "MainTransform")] private Transform mainTransform;
    [Inject(Id = "ModelTransform")] private Transform modelTransform;
    [Inject] private Rigidbody2D characterRigidbody;
    [Inject] private NavMeshAgent navMeshAgent;
    [Inject] private CombatSystem combatSystem;

    [SerializeField] private float speed = 3f;
    [SerializeField] private float rotationalSpeed = 240f;

    private Coroutine moveCoroutine;
    private Coroutine rotateCoroutine;
    private Coroutine followCoroutine;
    private Vector3 pointToMove;
    private float moveAngle;
    private float speedAngleModifier;
    private Queue<Vector3> path = new Queue<Vector3>();
    private NavMeshPath navMeshPath;
    public Action<Queue<Vector3>> OnPathGenerated = delegate { };
    private Transform lookTarget;

    private double distanceCheckValue = Vector3.one.sqrMagnitude * 0.01;

    private void Awake()
    {
        combatSystem.OnTargetChange += OnTargetChange;
    }

    private void Start()
    {
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        navMeshPath = new NavMeshPath();
    }

    private void OnDestroy()
    {
        combatSystem.OnTargetChange -= OnTargetChange;
    }

    private void OnTargetChange(Target target)
    {
        rotateCoroutine ??= StartCoroutine(RotateRoutine());
        lookTarget = target?.targetInfo.Transform;
    }

    public void Follow(Transform targetTransform)
    {
        if (followCoroutine != null) 
            StopCoroutine(followCoroutine);

        if (targetTransform != null) 
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
            if (combatSystem.Attack != null && !combatSystem.Attack.IsWithinDistance())
            {
                MoveCharacter(targetTransform.position);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void MoveCharacter(Vector3 point)
    {
        navMeshAgent.enabled = true;

        if (navMeshAgent.isActiveAndEnabled && !navMeshAgent.CalculatePath(point, navMeshPath)) return;
        path = new Queue<Vector3>(navMeshPath.corners);

        navMeshAgent.enabled = false;

        OnPathGenerated(path);

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
        while (path.Count > 0 || (combatSystem.IsHardLock && combatSystem.LockTarget != null) /*&& !combatSystem.Attack.IsWithinDistance()*/)
        {
            pointToMove = path.Count <= 1 && combatSystem.IsHardLock
            ? combatSystem.LockTarget.targetInfo.Transform.position
            : path.Peek();

             var moveDirection = pointToMove - modelTransform.position;
            moveAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;

            if (rotateCoroutine == null)
            {
                rotateCoroutine = StartCoroutine(RotateRoutine());
            }

            if (path.Count <= 1 && combatSystem.IsHardLock)
            {
                yield return MoveToAttackRange();
            }
            else
            {
                yield return MoveToPointRoutine();
                path.Dequeue();
            }

            yield return null;
        }
        moveCoroutine = null;
    }

    IEnumerator MoveToPointRoutine()
    {
        while ((mainTransform.position - pointToMove).sqrMagnitude > distanceCheckValue)
        {
            characterRigidbody.velocity = speed
            * speedAngleModifier
            * (pointToMove - mainTransform.position).normalized;

#if UNITY_EDITOR
            var rayColor = Color.green;
            rayColor.a = 0.16f;
            Debug.DrawRay(mainTransform.position, pointToMove - mainTransform.position, rayColor);
#endif

            yield return null;
        }
    }

    IEnumerator MoveToAttackRange()
    {
        while (combatSystem.LockTarget != null && !combatSystem.Attack.IsWithinDistance())
        {
            characterRigidbody.velocity = speed
            * speedAngleModifier
            * (combatSystem.LockTarget.targetInfo.Transform.position - mainTransform.position).normalized;

#if UNITY_EDITOR
            var rayColor = Color.green;
            rayColor.a = 0.16f;
            Debug.DrawRay(mainTransform.position,
                combatSystem.LockTarget.targetInfo.Transform.position - mainTransform.position, 
                rayColor);
#endif

            yield return null;
        }
    }

    IEnumerator RotateRoutine()
    {
        float angleBetween = float.MaxValue;

        while (lookTarget != null || (moveCoroutine != null && angleBetween > 0.01f))
        {
            Quaternion lookRotation;
            if (lookTarget != null)
            {
                var target = lookTarget.position - mainTransform.position;
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