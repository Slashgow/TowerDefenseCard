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
        Timer.Cancel(craftingTimer);
        //Debug.Log($"Destroy {this.transform.GetInstanceID()}");
    }

    public void Init(Transform p_stackParent, float p_craftingDelay, float p_cooldownBarOffset, int p_craftID)
    {
        stackParent = p_stackParent;
        craftingDelay = p_craftingDelay;
        fillImage.fillAmount = 0;
        cooldownBarOffset = p_cooldownBarOffset;
        craftID = p_craftID;


        craftingTimer = Timer.Register(
                    duration: craftingDelay,
                    onUpdate: secondsElapsed => UpdateCooldownBar(secondsElapsed),
                    onComplete: () =>
                    {
                        OnCraftDelayEnd?.Invoke(craftID);
                        Destroy(this.gameObject);
                    });
    }

    private void OnCooldownCraftCancel(int craftID)
    {
        if(craftID != this.craftID)
            return;

        Timer.Cancel(craftingTimer);
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
