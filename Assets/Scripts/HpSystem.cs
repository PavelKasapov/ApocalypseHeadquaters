using System;
using UnityEngine;

public class HpSystem : MonoBehaviour, IHittable
{
    [SerializeField] private float _hpValue;
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
    
    public void TakeDamage(IDamageMaker damageMaker)
    {
        Hp -= damageMaker.Damage;
    }
}