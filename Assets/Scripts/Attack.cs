using System;
using System.Collections;
using UnityEngine;

public abstract class Attack
{
    public abstract float MaxAttackRange { get; protected set; }
    
    protected abstract float delayBeforeAttack { get; set; }
    protected abstract float delayAfterAttack { get; set; }
    private WaitForSeconds waitBeforeAttack;
    private WaitForSeconds waitAfterAttack;

    protected readonly SightSystem sightSystem;
    protected readonly Transform selfTransform;
    protected Target attackTarget;
    protected float CurrentStatusDuration;
    private Coroutine tryAttackCoroutine;
    private AttackStatus _status;
    public Action<AttackStatus, float> OnStatusChange;
    public AttackStatus Status 
    {
        get => _status;
        set 
        {
            if (_status != value)
            {
                _status = value;
                OnStatusChange(value, CurrentStatusDuration);
            }
        }
    }
    
    public Attack(
        Transform selfTransform,
        SightSystem sightSystem)
    {
        this.selfTransform = selfTransform;
        this.sightSystem = sightSystem;
    }

    public virtual bool CanAttack(Target target)
    {
        return target != null 
            && sightSystem.VisibleTargets.ContainsKey(target.targetInfo) 
            && IsWithinDistance(target);
    }
    public bool CanAttack() => CanAttack(attackTarget);

    public bool IsWithinDistance(Target target)
    {
        if (target == null)
        {
            return false;
        }
        target.UpdateDistance();
        return target.Distance < MaxAttackRange;
    }
    public bool IsWithinDistance() => IsWithinDistance(attackTarget);

    public virtual void ChangeTarget(Target target)
    {
        if (this.attackTarget == target 
            || !selfTransform.gameObject.activeInHierarchy) return;

        if (Status == AttackStatus.Aiming) Stop();
        Perform(target);
    }

    public void Stop()
    {
        if (tryAttackCoroutine != null) sightSystem.StopCoroutine(tryAttackCoroutine);
        tryAttackCoroutine = null;
    }

    public void Perform(Target target)
    {
        if (target == null) return;

        attackTarget = target;

        tryAttackCoroutine ??= sightSystem.StartCoroutine(TryAttackRoutine());
    }

    protected virtual IEnumerator AttackRoutine()
    {
        while (tryAttackCoroutine != null 
            && attackTarget != null 
            && CanAttack(attackTarget))
        {
            CurrentStatusDuration = delayBeforeAttack;
            Status = AttackStatus.Aiming;
            yield return waitBeforeAttack ??= new WaitForSeconds(delayBeforeAttack);

            yield return SingleAttackRoutine();

            CurrentStatusDuration = delayAfterAttack;
            Status = AttackStatus.AttackCooldown;
            yield return waitAfterAttack ??= new WaitForSeconds(delayAfterAttack);   
        }
        CurrentStatusDuration = float.Epsilon;
        Status = AttackStatus.NotPossible;
    }

    protected abstract IEnumerator SingleAttackRoutine();

    IEnumerator TryAttackRoutine()
    {
        while (attackTarget != null)
        {
            yield return AttackRoutine();
            yield return new WaitForFixedUpdate();
        }
        tryAttackCoroutine = null;
    }
}
