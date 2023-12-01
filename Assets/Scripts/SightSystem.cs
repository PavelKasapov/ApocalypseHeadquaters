using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class SightSystem : MonoBehaviour
{
    private const float ModelOffset = 0.25f;

    [SerializeField] private ViewField viewField;
    [SerializeField] public LayerMask whatBlocksDirectView;

    [Inject(Id = "ModelTransform")] private Transform modelTransform;

    public EntityType entityType;
    private List<Target> targetsInCollider = new();
    public IReadOnlyDictionary<ITargetInfo, Target> VisibleTargets => targetsInCollider.Where(target => target.IsDirectVision).ToDictionary(target => target.targetInfo);

    public Action OnTargetChange = delegate { };
    public Action<Target, bool> OnVisionChange = delegate { };

    public void Initialize(int sightAngle, float sightDistance, EntityType entityType)
    {
        viewField.SetAnglesAndDistance(sightAngle, sightDistance, 2f);
        this.entityType = entityType;
    }

    private void OnDisable()
    {
        foreach (var target in VisibleTargets.Values)
        {
            target.IsDirectVision = false;
        }
        foreach (var target in targetsInCollider)
        {
            OnVisionChange?.Invoke(target, false);
        }
        targetsInCollider.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActiveAndEnabled && collision.TryGetComponent<ITargetInfo>(out var targetInfo) 
            && targetInfo.EntityType != entityType)
        {
            var target = new Target(targetInfo, this);

            targetsInCollider.Add(target);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isActiveAndEnabled && collision.TryGetComponent<ITargetInfo>(out var targetInfo) 
            && targetInfo.EntityType != entityType)
        {
            var target = targetsInCollider.First(target => target.targetInfo == targetInfo);
            
            targetsInCollider.Remove(target);

            if (target.IsDirectVision)
            {
                OnVisionChange?.Invoke(target, false);
            }
        }
    }

    private void FixedUpdate()
    {
        foreach (var target in targetsInCollider)
        {
            var isDirectVision = CheckDirectSight(target.targetInfo.Transform);

            if (isDirectVision != target.IsDirectVision) 
            {
                target.IsDirectVision = isDirectVision;
                OnVisionChange?.Invoke(target, isDirectVision);
            }
        }
    }

    public float GetDistance(Vector3 targetPosition)
    {
        return Vector3.Distance(modelTransform.position, targetPosition);
    }

    public bool CheckDirectSight(Transform targetTransform)
    {
        if (!targetsInCollider.Any(target => target.targetInfo.Transform == targetTransform))
            return false;

        var direction = (targetTransform.position - modelTransform.position).normalized;
        var modelWithOffset = modelTransform.position + direction * ModelOffset;
        var rayVector = targetTransform.position - modelWithOffset;

        var hit = Physics2D.Raycast(
            modelWithOffset,
            direction,
            rayVector.magnitude,
            whatBlocksDirectView);

        var isDirectVision = hit.transform == null;

#if UNITY_EDITOR
        var rayColor = isDirectVision ? Color.red : Color.yellow;
        rayColor.a = 0.08f;
        Debug.DrawRay(modelWithOffset,
            isDirectVision ? rayVector : (Vector3)hit.point - modelWithOffset,
            rayColor);
#endif

        return isDirectVision;
    }
}