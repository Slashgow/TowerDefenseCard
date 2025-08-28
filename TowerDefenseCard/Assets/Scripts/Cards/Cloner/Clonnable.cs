using UnityEngine;
using UnityTimer;

public class Clonnable : MonoBehaviour
{
    private Cloner parentSystem;
    private float lifetime;
    private Timer lifetimeTimer;
    public bool IsClone => true;
    public float RemainingLifetime => lifetimeTimer != null ? lifetimeTimer.GetTimeRemaining() : 0f;

    public void Initialize(Cloner parent, float life)
    {
        parentSystem = parent;
        lifetime = life;

        lifetimeTimer = Timer.Register(lifetime, () =>
        {
            if (parentSystem != null)
            {
                parentSystem.DestroyClone(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        });
    }



    private void OnDestroy()
    {
        if (lifetimeTimer != null && !lifetimeTimer.isDone)
        {
            lifetimeTimer.Cancel();
        }
    }
}
