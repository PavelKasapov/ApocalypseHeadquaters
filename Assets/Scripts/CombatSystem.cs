using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Zenject;

public class CombatSystem : IInitializable, IDisposable
{
    private static readonly WaitForSeconds WaitOneSecond = new(1);

    [Inject] private SightSystem sightSystem;
    [Inject(Optional = true)] private RangedAttack rangedAttack;
    [Inject] private MeleeAttack meleeAttack;
    [Inject] private BehaviorColorIndicator behaviorColorIndicator;
    [Inject] private TargetAimDrawer targetAimDrawer;
    [Inject(Optional = true)] private SquadSight squadSight;

    public Action<AttackStatus, float> OnAttackStatusChange = delegate { };

    private Coroutine targetRoutine;
    private Coroutine attackCoroutine;
    private Coroutine delayedReprioritizeCoroutine;

    private Target _lockTarget;
    private Attack _attack;
    
    public Action<Target> OnTargetChange = delegate { };
    public bool IsHardLock { get; set; }
    public Target LockTarget
    {
        get => _lockTarget;
        set
        {
            if (_lockTarget != value)
            {
                
                _lockTarget = value;
                OnTargetChange(value);
                _attack?.ChangeTarget(value);

                if (value != null && attackCoroutine == null)
                {
                    attackCoroutine = sightSystem.StartCoroutine(AttackRoutine());
                }

                if (value == null)
                {
                    Debug.Log(value);
                }
            }
        }
    }
    public Attack Attack
    {
        get => _attack; 
        set
        {
            if (value != _attack)
            {
                _attack?.Stop();
                _attack = value;
                _attack.OnStatusChange = OnAttackStatusChange;
                _attack.Perform(LockTarget);
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

    IEnumerator AttackRoutine()
    {
        while (LockTarget != null)
        {
            if (Attack?.Status != AttackStatus.Attacking)
            {
                Attack = rangedAttack == null || LockTarget.Distance < meleeAttack.MaxAttackRange
                    ? meleeAttack
                    : rangedAttack;
            }

            targetAimDrawer.SetTarget(Attack.Status != AttackStatus.NotPossible ? LockTarget : null);
            behaviorColorIndicator.SetColor(Attack.Status != AttackStatus.NotPossible ? Color.red : Color.yellow);

            yield return new WaitForFixedUpdate();
        } 

        targetAimDrawer.SetTarget(null);
        behaviorColorIndicator.SetColor(Color.green);

        attackCoroutine = null;
    }

    public void HardLockTarget(ITargetInfo targetInfo)
    {
        if (targetInfo != null)
        {
            var target = squadSight != null
            ? squadSight.VisibleTargets.FirstOrDefault(target => target.targetInfo == targetInfo)
            : sightSystem.VisibleTargets[targetInfo];

            if (target != null) // TODO: Try to rework click system to not work on enemies that are not in sight.
            {
                LockTarget = target;
                IsHardLock = true;
            }
        }
        else
        {
            RemoveHardLock();
        }
    }

    private void RemoveHardLock()
    {
        if (IsHardLock)
        {
            IsHardLock = false;
            ReprioritizeTargets();
        }
    }

    public void ReprioritizeTargets()
    {
        if (!IsHardLock)
        {
            if (Attack?.Status == AttackStatus.Attacking && delayedReprioritizeCoroutine == null)
            {
                delayedReprioritizeCoroutine = sightSystem.StartCoroutine(DelayedReprioritizeRoutine());
                return;
            }

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
    }

    IEnumerator DelayedReprioritizeRoutine()
    {
        yield return new WaitUntil(() => Attack?.Status != AttackStatus.Attacking);
        delayedReprioritizeCoroutine = null;
        ReprioritizeTargets();
    }
}