using System;
using UnityEngine;

public class HpSystem
{
    private float _hpValue;
    private readonly GameObject gameObject;
    public Action<float> onHpChanged = delegate { };
    public float Hp {
        get => _hpValue;
        private set 
        {
            _hpValue = value;
            onHpChanged.Invoke(value);

            if (_hpValue <= 0)
            {
                gameObject.SetActive(false);
            }
        } 
    }

    public HpSystem(Transform characterTransform, float hpValue) 
    {
        gameObject = characterTransform.gameObject;
        _hpValue = hpValue;
    }

    public void TakeDamage(IDamageMaker damageMaker)
    {
        Hp -= damageMaker.Damage;
    }
}