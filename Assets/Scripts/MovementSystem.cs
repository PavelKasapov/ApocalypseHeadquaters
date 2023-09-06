using System;
using System.Collections;
using UnityEngine;

public class MovementSystem : MonoBehaviour
{
    //private readonly ICoroutineHandler coroutineHandler;
    [SerializeField] private SightSystem sightSystem;
    [SerializeField] private new Transform transform;
    [SerializeField] private Transform modelTransform;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float rotationalSpeed = 240f;
    [SerializeField] private LineRenderer pathRenderer;

    //[SerializeField] SightSystem sight;

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
    /*public MovementSystem(
        //ICoroutineHandler coroutineHandler,
        SightSystem sightSystem,
        Transform transform,
        Transform modelTransform,
        float speed = 3f,
        float rotationalSpeed = 240f)
    {
        //this.coroutineHandler = coroutineHandler;
        this.sightSystem = sightSystem;
        this.transform = transform;
        this.modelTransform = modelTransform;
        this.speed = speed;
        this.rotationalSpeed = rotationalSpeed;
    }*/

    public void MoveCharacter(Vector3 point)
    {
        var moveDirection = point - modelTransform.position;
        moveAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        pointToMove = point;
        if (moveCoroutine == null)
        {
            moveCoroutine = StartCoroutine(MoveRoutine());
        }
        rotateCoroutine ??= StartCoroutine(RotateRoutine());
    }

    IEnumerator MoveRoutine()
    {
        pathRenderer.enabled = true; 
        while ((transform.position - pointToMove).sqrMagnitude > distanceCheckValue)
        {
            pathRenderer.SetPosition(0, transform.position);
            pathRenderer.SetPosition(1, pointToMove);
            transform.Translate((pointToMove - transform.position).normalized * speed * speedAngleModifier * Time.deltaTime);

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
                var target = sightSystem.MainTarget.Transform.position - transform.position;
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
