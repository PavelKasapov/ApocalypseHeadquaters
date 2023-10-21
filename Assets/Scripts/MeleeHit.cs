public class MeleeHit : IDamageMaker
{
    private float damage;
    public float Damage => damage;
    public MeleeHit(float damage = 1f)
    {
        this.damage = damage;
    }
}