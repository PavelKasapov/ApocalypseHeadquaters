using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemySpawner : MonoBehaviour
{
    [Inject] private Enemy.Factory _enemyFactory;

    private List<Enemy> enemies = new List<Enemy>();
    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy(Vector3 position)
    {
        var enemy = _enemyFactory.Create();

        enemy.transform.position = position;
        enemy.transform.SetParent(transform);

        enemies.Add(enemy);
    }

    private void SpawnEnemy()
    {
        var directionDegrees = Random.Range(0, 360);
        var position = new Vector3(Mathf.Sin(directionDegrees) * 10, Mathf.Cos(directionDegrees) * 10);

        SpawnEnemy(position);
    }
}
