using System;
using UnityEngine;
using Zenject;

public class HpSystem
{
    private float _hpValue = 30;
    private readonly GameObject characterGameObject;
    public Action<float> OnHpChanged = delegate { };
    public float Hp {
        get => _hpValue;
        private set 
        {
            _hpValue = value;
            OnHpChanged.Invoke(value);

            if (_hpValue <= 0)
            {
                characterGameObject.SetActive(false);
            }
        } 
    }

    public HpSystem([Inject(Id="MainTransform")]Transform mainTransform) 
    {
        characterGameObject = mainTransform.gameObject;
    }

    public void TakeDamage(IDamageMaker damageMaker)
    {
        Hp -= damageMaker.Damage;
    }
}