using UnityEngine;
using UnityEngine.UIElements;

public class InGameController : MonoBehaviour
{
    [SerializeField] private CraftingManager craftingManager;

    private void OnEnable()
    {
        VisualElement visualElement = GetComponent<UIDocument>().rootVisualElement;
        visualElement.Q<ProgressBar>("ProgressBarCraftMode").dataSource = craftingManager;
    }
}
