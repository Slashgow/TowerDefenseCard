using UnityEngine;

public class AnimationEnableEffect : Effect 
{
    [SerializeField] private GameObject effectPrefab;

    public override void DoEffect()
    {
        GameObject effectInstance = Instantiate(effectPrefab, this.transform);
        effectInstance.transform.localPosition = Vector3.zero;
        effectInstance.transform.localRotation = Quaternion.identity;
    }
}
