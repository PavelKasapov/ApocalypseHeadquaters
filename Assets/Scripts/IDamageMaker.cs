using UnityEngine;

public interface IDamageMaker
{
    public float Damage { get; }
    public Vector3 Direction { get; }
}