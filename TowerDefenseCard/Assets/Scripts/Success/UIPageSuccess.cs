using System;
using System.Collections.Generic;
using UnityEngine;

public class UIPageSuccess : UIPage
{
    [SerializeField] private GameObject uiSuccessPrefab;
    public GameObject UISuccessPrefab => uiSuccessPrefab;

    [SerializeField] private Transform contentScrollView;

    public event Action<Card> OnSelectSuccess;
    public override void Show()
    {
        base.Show();
        CardUtility.DestroyAllChildren(contentScrollView);

        List<SuccessData> successDatas = 

        
    }

   
    public override void Hide()
    {
        base.Hide();


    }
}
