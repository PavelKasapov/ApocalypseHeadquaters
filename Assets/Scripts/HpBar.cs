using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HpBar : MonoBehaviour
{
    [SerializeField] Slider _hpSlider;

    [Inject] Character �haracter;

    private void Start()
    {
        if (�haracter.HpSystem != null)
        {
            InitSlider(�haracter.HpSystem);
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

    private void OnDestroy()
    {
        �haracter.HpSystem.onHpChanged -= SetValue;
    }
}
