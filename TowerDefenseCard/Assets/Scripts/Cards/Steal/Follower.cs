using UnityEngine;

public class Follower : MonoBehaviour
{
    private Vector3 offset;
    private Transform transformToFollow;
    private bool isFollowing = false;
    public void SetupFollower(Transform transformToFollow, Vector3 offset)
    {
        this.transformToFollow = transformToFollow;
        this.offset = offset;
        isFollowing = true;
    }

    private void Update()
    {
        if(!isFollowing)
            return;

        this.transform.position = transformToFollow.position + offset;
        this.transform.rotation = transformToFollow.rotation;
    }

    private void OnDisable() => isFollowing = false;
}
