using UnityEngine;
using Zenject;

public class HitTaker : MonoBehaviour, IHittable
{
    [Inject] private HpSystem hpSystem;
    public void TakeDamage(IDamageMaker damageMaker) => 
        hpSystem.TakeDamage(damageMaker);
}