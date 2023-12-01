using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField] private InfoPanel infoPanel;
    public override void InstallBindings()
    {
        Container.BindInstance(infoPanel);
    }
}