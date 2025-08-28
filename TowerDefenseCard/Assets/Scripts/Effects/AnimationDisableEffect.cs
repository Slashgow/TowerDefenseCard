using UnityEngine;

public class AnimationDisableEffect : Effect 
{
    [SerializeField] private GameObject effectPrefab;

    private void OnDisable()
    {
        DoEffect();
    }
    public override void DoEffect()
    {
        GameObject effectInstance = Instantiate(effectPrefab, this.transform.position, Quaternion.identity);
    }
}
