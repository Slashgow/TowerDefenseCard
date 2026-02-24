using System;
using UnityEngine;
using UnityEngine.UI;
using UnityTimer;


public class CooldownBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    private float craftingDelay;
    private Transform stackParent;
    private float cooldownBarOffset;
    private int craftID;

    private Timer craftingTimer;

    public event Action<int> OnCraftDelayEnd;

    private void OnEnable()
    {
        fillImage.fillAmount = 0f;
        CraftingManager.Instance.OnCraftCancel += OnCooldownCraftCancel;
    }

    private void OnDisable()
    {
        if(CraftingManager.HasInstance)
            CraftingManager.Instance.OnCraftCancel -= OnCooldownCraftCancel;
    }

    private void OnDestroy()
    {
        craftingTimer?.Cancel();
        //Debug.Log($"Destroy {this.transform.GetInstanceID()}");
    }

    public void Init(Transform p_stackParent, float p_craftingDelay, float p_cooldownBarOffset, int p_craftID, float p_elapsedTime = 0f)
    {
        stackParent = p_stackParent;
        craftingDelay = p_craftingDelay;
        fillImage.fillAmount = 0;
        cooldownBarOffset = p_cooldownBarOffset;
        craftID = p_craftID;

        fillImage.fillAmount = p_elapsedTime / p_craftingDelay;

        craftingTimer = Timer.Register(
                    duration: p_craftingDelay - p_elapsedTime,
                    onUpdate: secondsElapsed => UpdateCooldownBar(secondsElapsed + p_elapsedTime),
                    onComplete: () =>
                    {
                        OnCraftDelayEnd?.Invoke(craftID);
                        OnCraftDelayEnd = null;
                        Destroy(this.gameObject);
                    });
    }

    private void OnCooldownCraftCancel(int craftID)
    {
        if(craftID != this.craftID)
            return;

        craftingTimer?.Cancel();
        OnCraftDelayEnd = null;
        //Timer.Cancel(craftingTimer);
        //Debug.Log($"try cancel {this.transform.GetInstanceID()}");
        Destroy(this.gameObject);
    }
    private void UpdateCooldownBar(float currentTime)
    {
        transform.position = stackParent.position + new Vector3(0, cooldownBarOffset, 0);
        //Debug.Log($"current time {currentTime}");
        if (fillImage != null) 
            fillImage.fillAmount = currentTime / craftingDelay;
    }
}
