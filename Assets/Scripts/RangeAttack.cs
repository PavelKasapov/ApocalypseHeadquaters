using System.Collections;
using System.Linq;
using UnityEngine;
using Zenject;

public class RangeAttack : MonoBehaviour
{
    [Inject] private BulletPool bulletPool;
    [SerializeField] Sight enemyDetection;
    [SerializeField] Transform selfTransform;
    [SerializeField] LineRenderer lineRenderer;

    private ITarget _target;
    private Coroutine coroutine;

    public ITarget Target => _target;

    private void Awake()
    {
        enemyDetection.OnFirstAppear = () => coroutine = StartCoroutine(ShootingRoutine());
        enemyDetection.OnLastDisappear = () => { StopCoroutine(coroutine); coroutine = null; _target = null; };
    }

    IEnumerator ShootingRoutine()
    {
        while (true)
        {
            enemyDetection.sightTargetsList = enemyDetection.sightTargetsList.OrderBy(enemy => Vector3.Distance(enemy.Transform.position, transform.position)).ToList();
            _target = enemyDetection.sightTargetsList.FirstOrDefault();

            yield return new WaitForSeconds(1);

            if (_target != null)
            {
                var selfPosition = selfTransform.position;
                var targetPosition = _target.Transform.position;

                bulletPool.Pool.Get().Launch(selfPosition, (targetPosition - selfPosition).normalized);
            }
        }
    }

    private void Update()
    {
        if (_target != null)
        {
            lineRenderer.enabled = true;

            var selfPosition = selfTransform.position;
            var targetPosition = _target.Transform.position;

            lineRenderer.SetPosition(0, selfPosition);
            lineRenderer.SetPosition(1, targetPosition);
        }
        else 
        { 
            lineRenderer.enabled = false; 
        }
    }
}
