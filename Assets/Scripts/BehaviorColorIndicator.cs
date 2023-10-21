using System.Collections;
using UnityEngine;

public class BehaviorColorIndicator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    /*public static Color green = Color.green;
    public static Color yellow = Color.yellow;
    public static Color red = Color.red;*/

    Color prevColor;

    Coroutine delayedSetColorCoroutine;
    public void SetColor(Color color)
    {
        if (prevColor == color) return;
        prevColor = color;
        Debug.Log(color);

        if (delayedSetColorCoroutine != null ) StopCoroutine( delayedSetColorCoroutine );

        delayedSetColorCoroutine = StartCoroutine(DelayedSetColorRoutine(color));
    }

    IEnumerator DelayedSetColorRoutine(Color color)
    {
        yield return new WaitForSeconds(0.05f);
        spriteRenderer.color = color;
    }
}