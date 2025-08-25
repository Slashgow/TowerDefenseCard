using UnityEngine;
using UnityEngine.UI;

public class MoveBarUI : MonoBehaviour
{
    [SerializeField] private CombatMoveCondtionner combatMoveConditionner;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image fillImage;
    private void OnEnable()
    {
        CombatMoveConditionner_OnEndCooldown();

        combatMoveConditionner.OnStartCooldown += CombatMoveConditionner_OnStartCooldown;
        combatMoveConditionner.OnEndCooldown += CombatMoveConditionner_OnEndCooldown;
        combatMoveConditionner.OnTickCooldown += UpdateMoveBar;
    }

    private void OnDisable()
    {
        combatMoveConditionner.OnStartCooldown -= CombatMoveConditionner_OnStartCooldown;
        combatMoveConditionner.OnEndCooldown -= CombatMoveConditionner_OnEndCooldown;
        combatMoveConditionner.OnTickCooldown -= UpdateMoveBar;
    }

    private void UpdateMoveBar(float timeElapsed)
    {
        if (fillImage != null)
            fillImage.fillAmount = timeElapsed / combatMoveConditionner.CooldownDurationBetweenMoves;
    }

    private void CombatMoveConditionner_OnEndCooldown() => canvas.enabled = false;
    private void CombatMoveConditionner_OnStartCooldown() => canvas.enabled = true;
}
