using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HpBar : MonoBehaviour
{
    [SerializeField] Slider _hpSlider;

    [Inject] HpSystem hpSystem;

    private void Start()
    {
        if (hpSystem != null)
        {
            InitSlider(hpSystem);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void InitSlider(HpSystem hpSystem)
    {
        _hpSlider.maxValue = hpSystem.Hp;
        SetValue(hpSystem.Hp);
        hpSystem.onHpChanged += SetValue;
    }

    private void OnDestroy()
    {
        hpSystem.onHpChanged -= SetValue;
    }

    public void SetValue(float value) 
    {
        _hpSlider.value = value;
        gameObject.SetActive(_hpSlider.value != _hpSlider.maxValue);
    }
}