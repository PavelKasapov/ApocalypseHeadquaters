using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SightSystem : MonoBehaviour
{
    private static readonly WaitForSeconds WaitOneSecond = new(1);

    [SerializeField] private ViewField viewField;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private SpriteRenderer pointer;

    private EntityType entityType;
    public List<ITarget> sightTargetsList = new();
    public Action OnFirstAppear = delegate { };
    public Action OnLastDisappear = delegate { };
    public Action OnTargetChange = delegate { };
    public LineRenderer LineRenderer => lineRenderer;

    private ITarget _mainTarget;
    private Coroutine targetRoutine;

    public ITarget MainTarget
    {
        get => _mainTarget;
        private set
        {
            if (_mainTarget != value)
            {
                _mainTarget = value;
                OnTargetChange?.Invoke();
            }
        }
    }

    public void Initialize(EntityType entityType, int sightAngle, float sightDistance)
    {
        viewField.SetAnglesAndDistance(sightAngle, sightDistance);
        this.entityType = entityType;
    }

    IEnumerator CheckForTargetRoutine()
    {
        while (true)
        {
            sightTargetsList = sightTargetsList.OrderBy(enemy => Vector3.Distance(enemy.Transform.position, transform.position)).ToList();
            MainTarget = sightTargetsList.FirstOrDefault();

            yield return WaitOneSecond;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<ITarget>(out var target))
        {
            if (target.EntityType != entityType)
            {
                sightTargetsList.Add(target);

                if (sightTargetsList.Count == 1)
                {
                    OnFirstAppear.Invoke();
                    MainTarget = target;
                }

                if (sightTargetsList.Count == 2)
                {
                    targetRoutine = StartCoroutine(CheckForTargetRoutine());
                }
            }   
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<ITarget>(out var target))
        {
            if (target.EntityType != entityType)
            {
                sightTargetsList.Remove(target);

                if (sightTargetsList.Count == 0)
                {
                    OnLastDisappear.Invoke();
                    MainTarget = null;
                }

                if (sightTargetsList.Count == 1)
                {
                    MainTarget = sightTargetsList.First();

                    StopCoroutine(targetRoutine);
                    targetRoutine = null;
                }
            }
        }
    }

    //OnTargetSwitch todo
}
