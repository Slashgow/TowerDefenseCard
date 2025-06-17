using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Image))]
public class CooldownBarUI : MonoBehaviour
{
    private Image fillImage;

    private float craftingDelay;
    private Transform stackParent;
    private float cooldownBarOffset;

    private void Awake()
    {
        fillImage = GetComponent<Image>();
    }

    public void Init(Transform p_stackParent, float p_craftingDelay, float p_cooldownBarOffset)
    {
        stackParent = p_stackParent;
        craftingDelay = p_craftingDelay;
        fillImage.fillAmount = 0;
        cooldownBarOffset = p_cooldownBarOffset;

        CraftingManager.Instance.OnCooldownCraftCancel += OnCooldownCraftCancel;

        CraftingManager.Instance.OnCooldownCraftTick -= Instance_OnCooldownCraftTick;
        CraftingManager.Instance.OnCooldownCraftTick += Instance_OnCooldownCraftTick;
    }

    private void OnCooldownCraftCancel()
    {
        CraftingManager.Instance.OnCooldownCraftTick -= Instance_OnCooldownCraftTick;
    }

    private void Instance_OnCooldownCraftTick(float secondsElapsed)
    {
        transform.position = stackParent.position + new Vector3(0, cooldownBarOffset, 0);
        UpdateCooldownBar(secondsElapsed);
    }

    private void UpdateCooldownBar(float currentTime)
    {
        if (fillImage != null) 
            fillImage.fillAmount = currentTime / craftingDelay;
    }
}
