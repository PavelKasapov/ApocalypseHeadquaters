using UnityEngine;
using Zenject;

public class Ground : MonoBehaviour, IClickable
{
    public EntityType EntityType => EntityType.Ground;
}
