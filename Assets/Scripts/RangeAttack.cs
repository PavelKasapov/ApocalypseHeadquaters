using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class RangeAttack : IInitializable, IDisposable
{
    private readonly SightSystem sightSystem;
    private readonly BulletPool bulletPool;
    private readonly Transform selfTransform;

    private Coroutine shootingCoroutine;
    private Coroutine drawAimCoroutine;

    public RangeAttack(
        [Inject(Id = "ModelTransform")] Transform selfTransform,
        SightSystem sightSystem, 
        BulletPool bulletPool)
    {
        this.selfTransform = selfTransform;
        this.bulletPool = bulletPool;
        this.sightSystem = sightSystem;
    }

    public void Initialize() => sightSystem.OnTargetChange += OnTargetChange;
    public void Dispose() => sightSystem.OnTargetChange -= OnTargetChange;

    IEnumerator ShootingRoutine()
    {
        var target = sightSystem.MainTarget;
        while (sightSystem.MainTarget != null && sightSystem.MainTarget == target)
        {
            yield return new WaitForSeconds(1f);

            var selfPosition = selfTransform.position + selfTransform.up * 0.3f;
            var targetPosition = sightSystem.MainTarget.Transform.position;

            bulletPool.Pool.Get().Launch(selfPosition, (targetPosition - selfPosition).normalized);
        }
        shootingCoroutine = null;
    }

    IEnumerator DrawAimRoutine()
    {
        while (sightSystem.MainTarget != null)
        {
            var selfPosition = selfTransform.position;
            var targetPosition = sightSystem.MainTarget.Transform.position;

            sightSystem.LineRenderer.SetPosition(0, selfPosition);
            sightSystem.LineRenderer.SetPosition(1, targetPosition);

            yield return null;
        }
    }

    private void OnTargetChange()
    {
        if (!selfTransform.gameObject.activeInHierarchy) return;

        if (shootingCoroutine != null) sightSystem.StopCoroutine(shootingCoroutine);
        if (drawAimCoroutine != null) sightSystem.StopCoroutine(drawAimCoroutine);

        if (sightSystem.MainTarget != null)
        {
            shootingCoroutine = sightSystem.StartCoroutine(ShootingRoutine());
            drawAimCoroutine = sightSystem.StartCoroutine(DrawAimRoutine());
        }
    }
}
