using UnityEngine;

public class AnimationEnableEffect : Effect 
{
    [SerializeField] private GameObject effectPrefab;

    private void OnEnable() => DoEffect();
    public override void DoEffect() => Instantiate(effectPrefab, this.transform.position, Quaternion.identity);//, this.transform);
}
