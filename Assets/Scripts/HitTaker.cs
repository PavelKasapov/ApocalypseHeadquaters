using System;
using UnityEngine;
using Zenject;

public class HitTaker : MonoBehaviour
{
    [Inject] private HpSystem hpSystem;
    public Action<IDamageMaker> OnHit = delegate { };
    public void TakeDamage(IDamageMaker damageMaker)
    {
        hpSystem.TakeDamage(damageMaker);
        OnHit(damageMaker);
    }
}   