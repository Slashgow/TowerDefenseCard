using FirstGearGames.SmoothCameraShaker;
using UnityEngine;

public class CameraShaker : Effect
{
    [SerializeField] private ShakeData MyShake;
    public override void DoEffect()
    {
        if (!GameSettingsManager.Instance.IsScreenShakeEnable)
            return;

        CameraShakerHandler.Shake(MyShake);
    }
}
