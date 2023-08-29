using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] Slider _hpSlider;
    [SerializeField] HpSystem _hpSystem;

    private void Start()
    {
        if (_hpSystem != null)
        {
            InitSlider(_hpSystem);
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

    public void SetValue(float value) 
    {
        _hpSlider.value = value;
        gameObject.SetActive(_hpSlider.value != _hpSlider.maxValue);
    }
}
