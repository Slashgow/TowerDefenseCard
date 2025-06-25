using UnityEngine;
using UnityEngine.UIElements;

public class InGameController : MonoBehaviour
{
    [SerializeField] private CraftingManager craftingManager;
    [SerializeField] private ShopManager shopManager;

    private void OnEnable()
    {
        VisualElement visualElement = GetComponent<UIDocument>().rootVisualElement;
        visualElement.Q<ProgressBar>("ProgressBarCraftMode").dataSource = craftingManager;
        visualElement.Q<TextElement>("InkCounterText").dataSource = shopManager;
    }
}
