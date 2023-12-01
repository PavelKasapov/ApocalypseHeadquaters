using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI statusTMP;

    Coroutine sliderAnimationCoroutine;

    public void SetStatusText(string statusText, float duration)
    {
        if (duration < 0.2f) return;

        statusTMP.text = statusText;

        if (sliderAnimationCoroutine != null) StopCoroutine(sliderAnimationCoroutine);
        sliderAnimationCoroutine = StartCoroutine(SliderAnimationRoutine(duration));
    }

    IEnumerator SliderAnimationRoutine(float duration)
    {
        slider.value = 0f;
        slider.maxValue = duration;

        float time = 0f;
        
        while (time < duration)
        {
            yield return true;
            slider.value = time;
            time += Time.deltaTime;
        }

        sliderAnimationCoroutine = null;
    }
}
