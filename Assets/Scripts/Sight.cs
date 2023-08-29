using System;
using System.Collections.Generic;
using UnityEngine;

public class Sight : MonoBehaviour
{
    public List<ITarget> sightTargetsList = new List<ITarget>();
    public Action OnFirstAppear = delegate { };
    public Action OnLastDisappear = delegate { };
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<ITarget>(out var target))
        {
            sightTargetsList.Add(target);

            if (sightTargetsList.Count == 1)
                OnFirstAppear.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<ITarget>(out var target))
        {
            sightTargetsList.Remove(target);

            if (sightTargetsList.Count == 0)
                OnLastDisappear.Invoke();
        }
    }
}
