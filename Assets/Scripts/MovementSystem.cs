using System;
using System.Collections;
using UnityEngine;

public class MovementSystem : MonoBehaviour
{
    [SerializeField] Transform modelTransform;
    [SerializeField] float speed = 10f;
    [SerializeField] private float _rotationalSpeed = 240f;
    [SerializeField] RangeAttack rangeAttack;

    private Coroutine moveCoroutine;
    private Coroutine rotateCoroutine;
    private Vector3 pointToMove;
    private float moveAngle;
    private float speedAngleModifier;

    private double distanceCheckValue = Vector3.one.sqrMagnitude * 0.01;

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
        while ((transform.position - pointToMove).sqrMagnitude > distanceCheckValue)
        {
            transform.Translate((pointToMove - transform.position).normalized * speed * speedAngleModifier * Time.deltaTime);

            yield return null;
        }
        moveCoroutine = null;
    }

    IEnumerator RotateRoutine()
    {
        float angleBetween = float.MaxValue;

        while (rangeAttack.Target != null || angleBetween > 0.01f)
        {
            Quaternion lookRotation;
            if (rangeAttack.Target != null)
            {
                var target = rangeAttack.Target.Transform.position - modelTransform.position;
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

            modelTransform.rotation = Quaternion.RotateTowards(modelTransform.rotation, lookRotation, Time.deltaTime * _rotationalSpeed);
            
            yield return null;
        }

        speedAngleModifier = 1f;
        rotateCoroutine = null;
    }
}
