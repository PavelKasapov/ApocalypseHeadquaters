using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Zenject;

public class HitTaker : MonoBehaviour, IHittable
{
    [Inject] private Character character;
    //private HpSystem _hpSystem;
    //private HpSystem HpSystem => _hpSystem ??= character.HpSystem;
    public void TakeDamage(IDamageMaker damageMaker) => character.HpSystem.TakeDamage(damageMaker);

   /* private void Awake()
    {
        _hpSystem = character.HpSystem;
    }*/
}
