using FirstGearGames.SmoothCameraShaker;
using UnityEngine;

public class CameraShaker : Effect
{
    [SerializeField] private ShakeData MyShake;
    public override void DoEffect() => CameraShakerHandler.Shake(MyShake);

}
