using UnityEngine;

public class Enemy : MonoBehaviour, ITarget
{
    [SerializeField] Transform selfTransform;
    public Transform Transform => selfTransform;
}
