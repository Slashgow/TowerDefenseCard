using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField, Range(0, 10f)] private float transitionTime;

    public Animator Animator => animator;
    public float TransitionTime => transitionTime;

    private void Start()
    {
        if (SceneLoader.HasInstance)
            SceneLoader.Instance.RegisterTransitionAnimator(animator, transitionTime);
    }

    private void OnDestroy()
    {
        if (SceneLoader.HasInstance)
            SceneLoader.Instance.UnregisterTransitionAnimator();
    }
}
