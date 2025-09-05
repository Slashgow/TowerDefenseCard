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

        successTitle.text = successData.Title.GetLocalizedString();
       
        if(successData.Sprite != null)
            successImage.sprite = successData.Sprite;
    }

}
