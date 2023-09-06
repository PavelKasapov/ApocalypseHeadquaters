using UnityEngine;

public class Enemy : MonoBehaviour, ITarget, IClickable
{
    [SerializeField] Transform selfTransform;
    public Transform Transform => selfTransform;
    public EntityType EntityType => EntityType.Enemy;
}
