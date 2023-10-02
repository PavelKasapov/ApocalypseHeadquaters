using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class SightSystem : MonoBehaviour
{
    private const float ModelOffset = 0.3f;

    [SerializeField] private ViewField viewField;
    [SerializeField] public LayerMask whatBlocksDirectView;

    [Inject(Id = "ModelTransform")] private Transform modelTransform;

    private EntityType entityType;
    private List<Target> targetsInCollider = new();
    public IReadOnlyDictionary<ITargetInfo, Target> VisibleTargets => targetsInCollider.Where(target => target.IsDirectVision).ToDictionary(target => target.targetInfo);

    public Action OnTargetChange = delegate { };
    public Action<Target, bool> OnVisionChange = delegate { };

    public void Initialize(int sightAngle, float sightDistance)
    {
        viewField.SetAnglesAndDistance(sightAngle, sightDistance);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<ITargetInfo>(out var targetInfo) 
            && targetInfo.EntityType != entityType)
        {
            targetsInCollider.Add(new Target(targetInfo, this));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<ITargetInfo>(out var targetInfo) 
            && targetInfo.EntityType != entityType)
        {
            var target = targetsInCollider.First(target => target.targetInfo == targetInfo);
            
            targetsInCollider.Remove(target);

            OnVisionChange?.Invoke(target, false);
        }
    }

    private void FixedUpdate()
    {
        foreach (var target in targetsInCollider.ToList())
        {
            if (!targetsInCollider.Contains(target))
            {
                continue;
            }

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

        var hit = Physics2D.Raycast(
            modelWithOffset,
            direction,
            float.PositiveInfinity,
            whatBlocksDirectView);

        var isDirectVision = hit.transform == targetTransform;

        Debug.DrawRay(modelWithOffset,
            targetTransform.position - modelWithOffset,
            isDirectVision ? Color.red : Color.yellow);

        return isDirectVision;
    }
}
