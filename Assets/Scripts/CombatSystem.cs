using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Zenject;

public class CombatSystem : IInitializable, IDisposable
{
    private static readonly WaitForSeconds WaitOneSecond = new(1);

    [Inject] private SightSystem sightSystem;
    [Inject] private RangedAttack attack;
    [Inject] private BehaviorColorIndicator behaviorColorIndicator;
    [Inject] private TargetAimDrawer targetAimDrawer;

    private Coroutine targetRoutine;
    private Coroutine checkTargetDistanceRoutine;

    private Target _lockTarget;
    public Action OnTargetChange = delegate { };
    public bool IsHardLock { get; private set; }
    public bool IsWithinDistance { get; private set; }
    public Target LockTarget
    {
        get => _lockTarget;
        set
        {
            if (_lockTarget != value)
            {
                _lockTarget = value;
                OnTargetChange?.Invoke();
            }
        }
    }

    public void Initialize()
    {
        sightSystem.OnVisionChange += OnVisionChange;
    }
    public void Dispose()
    {
        sightSystem.OnVisionChange -= OnVisionChange;
    }

    void OnVisionChange(Target target, bool isVisible)
    {
        if (target == LockTarget && !isVisible)
        {
            IsHardLock = false;
        }

        if (isVisible || target == LockTarget) 
            ReprioritizeTargets();
    }

    IEnumerator TargetCheckingRoutine()
    {
        while (sightSystem.VisibleTargets.Count > 1)
        {
            yield return WaitOneSecond;

            ReprioritizeTargets();
        }
        targetRoutine = null;
    }

    private void ReprioritizeTargets()
    {
        if (!IsHardLock) {

            var Targets = sightSystem.VisibleTargets.Values;

            foreach (var target in Targets)
            {
                target.UpdateDistance();
            }

            if (Targets.Count() > 1)
            {
                LockTarget = Targets.Aggregate((l, r) => l.Distance < r.Distance ? l : r);
                if (targetRoutine == null)
                {
                    targetRoutine = sightSystem.StartCoroutine(TargetCheckingRoutine());
                }
            }
            else
            {
                LockTarget = Targets.FirstOrDefault();
            }
        }
        if (checkTargetDistanceRoutine == null
            && LockTarget != null)
        {
            checkTargetDistanceRoutine = sightSystem.StartCoroutine(TargetDistanceCheckingRoutine());
        }
    }

    IEnumerator TargetDistanceCheckingRoutine()
    {
        IsWithinDistance = false;
        Target prevTarget = null;

        while (LockTarget != null)
        {
            LockTarget.UpdateDistance();
            
            var isWithinDistance =  LockTarget.Distance < attack.AttackRange && sightSystem.VisibleTargets.ContainsKey(LockTarget.targetInfo);

            if (isWithinDistance != IsWithinDistance || prevTarget != LockTarget)
            {
                behaviorColorIndicator.SetColor(isWithinDistance ? Color.red : Color.yellow);
                targetAimDrawer.SetTarget(LockTarget);

                attack.SetAttackTarget(isWithinDistance ? LockTarget : null);
                prevTarget = LockTarget;

                IsWithinDistance = isWithinDistance;
            }

            yield return new WaitForFixedUpdate();
        } 

        targetAimDrawer.SetTarget(LockTarget);
        attack.SetAttackTarget(LockTarget);
        behaviorColorIndicator.SetColor(Color.green);

        checkTargetDistanceRoutine = null;
    }

    public void ChaseAndAttack(ITargetInfo targetInfo)
    {
        if (targetInfo != null)
        {
            var target = sightSystem.VisibleTargets.ContainsKey(targetInfo) 
                ? sightSystem.VisibleTargets[targetInfo] 
                : new Target(targetInfo, sightSystem);

            LockTarget = target;
            IsHardLock = true;
            ReprioritizeTargets();
        }
        else
        {
            StopChasing();
        }
    }

    private void StopChasing()
    {
        if (IsHardLock)
        {
            IsHardLock = false;
            ReprioritizeTargets();
        }
    }
}
