using System.Collections;
using UnityEngine;
using Zenject;

public class RangeAttack : MonoBehaviour
{
    [Inject] private Character character;
    [Inject] private BulletPool bulletPool;

    [SerializeField] private Transform pointerTransform;
    [SerializeField] Transform selfTransform;

    private Coroutine shootingCoroutine;
    private Coroutine drawAimCoroutine;

    private void OnEnable()
    {
        character.SightSystem.OnTargetChange += OnTargetChange;
    }

    private void OnDisable()
    {
        character.SightSystem.OnTargetChange -= OnTargetChange;
    }

    IEnumerator ShootingRoutine()
    {
        var target = character.SightSystem.MainTarget;
        while (character.SightSystem.MainTarget != null && character.SightSystem.MainTarget == target)
        {
            yield return new WaitForSeconds(1f);

            var selfPosition = pointerTransform.position;
            var targetPosition = character.SightSystem.MainTarget.Transform.position;

            bulletPool.Pool.Get().Launch(pointerTransform.position, (targetPosition - selfPosition).normalized);
        }
        shootingCoroutine = null;
    }

    IEnumerator DrawAimRoutine()
    {
        character.SightSystem.LineRenderer.enabled = true;

        while (character.SightSystem.MainTarget != null)
        {
            var selfPosition = character.Transform.position;
            var targetPosition = character.SightSystem.MainTarget.Transform.position;

            character.SightSystem.LineRenderer.SetPosition(0, selfPosition);
            character.SightSystem.LineRenderer.SetPosition(1, targetPosition);

            yield return null;
        }

        character.SightSystem.LineRenderer.enabled = false;
    }

    private void OnTargetChange()
    {
        if (!gameObject.activeInHierarchy) return;

        if (shootingCoroutine != null) StopCoroutine (shootingCoroutine);
        if (drawAimCoroutine != null) StopCoroutine(drawAimCoroutine);

        shootingCoroutine = StartCoroutine(ShootingRoutine());
        drawAimCoroutine = StartCoroutine(DrawAimRoutine());
    }
}
