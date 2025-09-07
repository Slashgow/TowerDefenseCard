using System;
using System.Collections.Generic;
using UnityEngine;

public class UIPageSuccess : UIPage
{
    [SerializeField] private GameObject uiSuccessPrefab;
    public GameObject UISuccessPrefab => uiSuccessPrefab;

    [SerializeField] private Transform contentScrollView;

    public event Action<SuccessData> OnSelectSuccess;
    public override void Show()
    {
        base.Show();
        CardUtility.DestroyAllChildren(contentScrollView);

        List<SuccessData> successDatas = SuccessManager.Instance.AllSuccessData;

        foreach (SuccessData successData in successDatas)
        {
            GameObject uiSuccessGameObject = Instantiate(uiSuccessPrefab, contentScrollView);
            UISuccess uISuccess = uiSuccessGameObject.GetComponent<UISuccess>();

            if (uISuccess != null)
            {
                uISuccess.SetupUISuccess(successData);
                uISuccess.OnSelectEvent -= UISuccess_OnSuccessSelected;
                uISuccess.OnSelectEvent += UISuccess_OnSuccessSelected;
            }
        }

    }

    private void UISuccess_OnSuccessSelected(SuccessData successData)
    {
        OnSelectSuccess?.Invoke(successData);
    }


    public override void Hide()
    {
        base.Hide();


    }
}
