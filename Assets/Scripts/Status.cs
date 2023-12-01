using System.Collections;
using UnityEngine;

public class Status
{
    protected float duration;
    float timePassed;

    public Status(float duration)
    {
        this.duration = duration;
    }

    public IEnumerator StatusRoutine()
    {
        timePassed = 0f;
        while (timePassed < duration) 
        { 
            yield return new WaitForFixedUpdate();
            timePassed += Time.deltaTime;
        }
    }
}