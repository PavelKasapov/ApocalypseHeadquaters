using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField] private InfoPanel infoPanel;
    [SerializeField] private GameObject enemyPrefab;
    public override void InstallBindings()
    {
        Container.BindInstance(infoPanel);
        Container.BindFactory<Enemy, Enemy.Factory>().FromComponentInNewPrefab(enemyPrefab);
    }
}