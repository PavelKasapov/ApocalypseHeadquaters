using UnityEngine;
using UnityEngine.Pool;

public class BulletPool : MonoBehaviour
{
    public ObjectPool<Bullet> Pool;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform selfTransform;

    private void Start()
    {
        Pool = new ObjectPool<Bullet>(
            () => {
                var bullet = Instantiate(bulletPrefab, selfTransform).GetComponent<Bullet>();
                bullet.OnRelease = () => Pool.Release(bullet);
                return bullet;
            },
            bullet => bullet.gameObject.SetActive(true),
            bullet => bullet.gameObject.SetActive(false),
            bullet => Destroy(bullet.gameObject),
            true, 10, 20);
    }
}
