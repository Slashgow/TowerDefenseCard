using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UISuccess : MonoBehaviour, IUISelectable<SuccessData>
{
    [SerializeField] private TextMeshProUGUI successTitle;
    [SerializeField] private Image successImage;

    private SuccessData currentSuccessData;

    public event Action<SuccessData> OnSelectEvent;

    public SuccessData GetSelectableData() => currentSuccessData;
    public void OnSelect(SuccessData data) => OnSelectEvent?.Invoke(data);

    public void SetupUISuccess(SuccessData successData)
    {
        currentSuccessData = successData;

        successData.OnComplete -= OnCompleteSuccess;
        successData.OnComplete += OnCompleteSuccess;

        successTitle.text = successData.Title.GetLocalizedString();

        UpdateSprite(successData);

    }

    private void UpdateSprite(SuccessData successData)
    {
        if (successData.isDone)
        {
            if (successData.SpriteUnlocked != null)
                successImage.sprite = successData.SpriteUnlocked;
        }
        else
        {
            if (successData.SpriteLocked != null)
                successImage.sprite = successData.SpriteLocked;
        }
    }

    private void OnCompleteSuccess(SuccessData successData)
    {
        UpdateSprite(successData);
    }
}
