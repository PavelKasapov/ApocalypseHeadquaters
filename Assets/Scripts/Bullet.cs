using System;
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour, IDamageMaker
{
    [SerializeField] Transform selfTransform;
    [SerializeField] float speed = 1f;
    public float damage = 1f;
    public Action OnRelease;
    private Coroutine moveCoroutine;
    private static TimeSpan threeSeconds = TimeSpan.FromSeconds(3);

    public float Damage => damage;

    public void Launch(Vector3 position, Vector3 direction)
    {
        selfTransform.position = position;

        float angle = Vector3.Angle(Vector3.up, new Vector3(direction.x, direction.y, 0.0f));
        if (direction.x > 0.0f) { angle = -angle; angle = angle + 360; }

        selfTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        moveCoroutine = StartCoroutine(MoveForwardRoutine());
    }

    IEnumerator MoveForwardRoutine()
    {
        var startTime = DateTime.Now;
        while (DateTime.Now - startTime < threeSeconds)
        {
            selfTransform.Translate(Vector2.up * speed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
        
        if (isActiveAndEnabled)
        {
            OnRelease.Invoke();
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent<IHittable>(out var hitTarget))
        {
            hitTarget.TakeDamage(this);
        }

        if (isActiveAndEnabled)
        {
            OnRelease.Invoke();
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
    }
}

public interface IDamageMaker
{
    public float Damage { get; }
}
