using System.Collections;
using System.Linq;
using UnityEngine;
using Zenject;

public class AI : MonoBehaviour
{
    private static readonly WaitForSeconds WaitOneSecond = new(1);

    [Inject] private SightSystem sightSystem;
    [Inject] private CombatSystem combatSystem;

    private Coroutine targetRoutine;
    public bool IsHardLock { get; private set; }

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
        if (target == combatSystem.LockTarget && !isVisible)
        {
            IsHardLock = false;
        }

        if (isVisible || target == combatSystem.LockTarget)
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
        if (!IsHardLock)
        {

            var Targets = sightSystem.VisibleTargets.Values;

            foreach (var target in Targets)
            {
                target.UpdateDistance();
            }

            if (Targets.Count() > 1)
            {
                combatSystem.LockTarget = Targets.Aggregate((closestTarget, nextTarget) => closestTarget.Distance < nextTarget.Distance ? closestTarget : nextTarget);
                if (targetRoutine == null)
                {
                    targetRoutine = sightSystem.StartCoroutine(TargetCheckingRoutine());
                }
            }
            else
            {
                combatSystem.LockTarget = Targets.FirstOrDefault();
            }
        }
    }


    public void ChaseAndAttack(ITargetInfo targetInfo)
    {
        if (targetInfo != null)
        {
            var target = sightSystem.VisibleTargets.ContainsKey(targetInfo)
                ? sightSystem.VisibleTargets[targetInfo]
                : new Target(targetInfo, sightSystem);

            combatSystem.LockTarget = target;
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