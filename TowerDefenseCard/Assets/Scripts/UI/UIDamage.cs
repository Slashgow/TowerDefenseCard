
using UnityEngine;
using UnityTimer;

public class UIDamage : MonoBehaviour
{
    [SerializeField, Range(0f,5f)] private float lifeDuration = 2f;

    private PoolingSystem poolingSystem;

    public void Setup(PoolingSystem poolingSystem)
    {
        this.poolingSystem = poolingSystem;
        Timer.Register(lifeDuration, onComplete: () => poolingSystem.AddToPool(this.gameObject));
    }
}
