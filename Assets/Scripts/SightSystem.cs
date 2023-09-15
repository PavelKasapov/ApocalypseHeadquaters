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
    //[SerializeField] private SpriteRenderer pointer;

    private EntityType entityType;
    public List<ITarget> sightTargetsList = new();
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
/*
    private void OnDisable()
    {
        lineRenderer.enabled = false;
    }*/

    public void Initialize(EntityType entityType, int sightAngle, float sightDistance)
    {
        viewField.SetAnglesAndDistance(sightAngle, sightDistance);
        this.entityType = entityType;
    }

    IEnumerator TargetCheckingRoutine()
    {
        while (sightTargetsList.Count > 1)
        {
            yield return WaitOneSecond;

            ReprioritizeTargets();
        }
        targetRoutine = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<ITarget>(out var target))
        {
            if (target.EntityType != entityType)
            {
                sightTargetsList.Add(target);

                ReprioritizeTargets();

                if (targetRoutine == null && sightTargetsList.Count > 1)
                {
                    targetRoutine = StartCoroutine(TargetCheckingRoutine());
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

                if (MainTarget == target) 
                    ReprioritizeTargets();
            }
        }
    }

    private void ReprioritizeTargets()
    {
        sightTargetsList = sightTargetsList.OrderBy(enemy => Vector3.Distance(enemy.Transform.position, transform.position)).ToList();
        MainTarget = sightTargetsList.FirstOrDefault();
        lineRenderer.enabled = MainTarget != null;
        //Debug.Log($"{transform.parent.parent.parent.name} {MainTarget.Transform.name}");
    }
}
