using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Zenject;

public class CombatSystem : IInitializable, IDisposable
{
    private static readonly WaitForSeconds WaitOneSecond = new(1);

    [Inject] private SightSystem sightSystem;
    [Inject] private RangedAttack rangedAttack;
    [Inject] private MeleeAttack meleeAttack;
    [Inject] private BehaviorColorIndicator behaviorColorIndicator;
    [Inject] private TargetAimDrawer targetAimDrawer;
    //[Inject] private List<Attack> AvailableAttacks;

    public CombatStatus Status => (
        (LockTarget != null ? CombatStatus.LookAtTarget : CombatStatus.None) |
        (LockTarget != null && IsWithinDistance ? CombatStatus.Attacking : CombatStatus.None) |
        (LockTarget != null && !IsWithinDistance && IsHardLock ? CombatStatus.GettingCloser : CombatStatus.None)
        );

    private Coroutine targetRoutine;
    private Coroutine attackCoroutine;

    private Target _lockTarget;
    private Attack _attack;
    
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
                _attack?.ChangeTarget(value);

                if (value != null && attackCoroutine == null)
                {
                    attackCoroutine = sightSystem.StartCoroutine(AttackRoutine());
                }
            }
        }
    }
    private Attack Attack
    {
        get => _attack; set
        {
            if (value != _attack)
            {
                _attack?.Stop();
                _attack = value;
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
    }

    IEnumerator AttackRoutine()
    {
        while (LockTarget != null)
        {
            if (Attack?.Status != AttackStatus.Attacking)
            {
                /*Attack = AvailableAttacks.FirstOrDefault(attack =>
                attack is RangedAttack ranged &&
                attack.CanAttack(LockTarget)) ??
                AvailableAttacks.FirstOrDefault(attack => attack is MeleeAttack);*/
                Attack = rangedAttack.CanAttack(LockTarget) && LockTarget.Distance > meleeAttack.MaxAttackRange
                    ? rangedAttack 
                    : meleeAttack;
            }
            targetAimDrawer.SetTarget(Attack.Status != AttackStatus.NotPossible ? LockTarget : null);

            behaviorColorIndicator.SetColor(Attack.Status != AttackStatus.NotPossible ? Color.red : Color.yellow);

            yield return new WaitForFixedUpdate();
        } 

        targetAimDrawer.SetTarget(null);
        behaviorColorIndicator.SetColor(Color.green);

        attackCoroutine = null;
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