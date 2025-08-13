using UnityEngine;

public class DestroySelfEffect : MonoBehaviour
{
    [SerializeField] private bool destroyOnDisable = false;

    private void OnDisable()
    {
        DestroySelf();
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
