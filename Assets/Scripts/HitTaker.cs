using UnityEngine;
using Zenject;

public class HitTaker : MonoBehaviour, IHittable
{
    [Inject] private Character character;
    public void TakeDamage(IDamageMaker damageMaker) => 
        character.HpSystem.TakeDamage(damageMaker);
}
