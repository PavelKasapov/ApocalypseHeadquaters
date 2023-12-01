using System;
using Zenject;

public class EnemyAI : IInitializable, IDisposable
{
    [Inject] private SightSystem sightSystem;
    [Inject] private CombatSystem combatSystem;
    [Inject] private MovementSystem movementSystem;
    [Inject] private HitTaker hitTaker;

    Target lastDissapearedTarget;

    public void Initialize()
    {
        combatSystem.OnTargetChange += OnTargetChange;
        sightSystem.OnVisionChange += OnVisionChange;
        hitTaker.OnHit += OnHit;
    }
    public void Dispose()
    {
        combatSystem.OnTargetChange -= OnTargetChange;
        sightSystem.OnVisionChange -= OnVisionChange;
        hitTaker.OnHit -= OnHit;
    }
    void OnTargetChange(Target target)
    {
        movementSystem.Follow(target?.targetInfo.Transform);
        
        if (target == null 
            && lastDissapearedTarget != null 
            && lastDissapearedTarget.targetInfo.Transform.gameObject.activeInHierarchy)
        {
            movementSystem.MoveCharacter(lastDissapearedTarget.targetInfo.Transform.position);
        }
    }

    void OnVisionChange(Target target, bool isVisible)
    {
        if (!isVisible && target.targetInfo.Transform.gameObject.activeInHierarchy)
        {
            lastDissapearedTarget = target;
        }
    }

    void OnHit(IDamageMaker damageMaker)
    {
        if (combatSystem.LockTarget == null && sightSystem.isActiveAndEnabled) 
            movementSystem.MoveCharacter(sightSystem.transform.position - damageMaker.Direction);
    }
}