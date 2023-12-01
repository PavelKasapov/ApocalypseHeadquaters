using UnityEngine;

public class MeleeHit : IDamageMaker
{
    private float damage;
    public float Damage => damage;

    public Vector3 Direction { get; private set; }
    public MeleeHit(Vector3 direction, float damage = 1f)
    {
        this.damage = damage;
        Direction = direction;
    }
}