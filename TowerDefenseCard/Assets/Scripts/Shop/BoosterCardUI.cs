using TMPro;
using UnityEngine;

public class BoosterCardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI remainingCardText;
    [SerializeField] private Booster booster;

    private void Start()
    {
        remainingCardText.text = booster.RemainingCards.ToString();
        booster.OnOpenBoosterUnity.AddListener(OnOpenBooster);
    }
    private void OnDestroy() => booster.OnOpenBoosterUnity.RemoveListener(OnOpenBooster);
    private void OnOpenBooster() => remainingCardText.text = booster.RemainingCards.ToString();
}
