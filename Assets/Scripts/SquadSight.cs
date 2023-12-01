using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class SquadSight : IInitializable, IDisposable
{
    [Inject] SightSystem sightSystem;
    private static List<SightSystem> sightSystems = new();
    private static List<Target> visibleCharacters = new();

    public IReadOnlyList<Target> VisibleTargets => visibleCharacters;

    public void Initialize()
    {
        sightSystems.Add(sightSystem);
        sightSystem.OnVisionChange += OnVisionChange;
    }

    public void Dispose()
    {
        sightSystems.Remove(sightSystem);
        sightSystem.OnVisionChange -= OnVisionChange;
    }

    private void OnVisionChange(Target target, bool isVisible)
    {
        if (isVisible)
        {
            if (!visibleCharacters.Contains(target))
            {
                visibleCharacters.Add(target);
                target.targetInfo.Transform.GetComponent<CharacterVisibility>().ChangeVisibility(isVisible);
            }
        }
        else 
        {
            if (!sightSystems.Any(system => system.VisibleTargets.ContainsKey(target.targetInfo)))
            {
                visibleCharacters.Remove(target);
                target.targetInfo.Transform.GetComponent<CharacterVisibility>().ChangeVisibility(isVisible);
            }
        }
    }
} 
