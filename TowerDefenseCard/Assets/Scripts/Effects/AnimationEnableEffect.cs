using UnityEngine;

public class AnimationEnableEffect : MonoBehaviour 
{
    [SerializeField] private GameObject effectPrefab;

    private void OnEnable()
    {
        Instantiate(effectPrefab, this.transform.position, Quaternion.identity);//, this.transform);
    }
}
