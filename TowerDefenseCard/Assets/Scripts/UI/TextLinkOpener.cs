using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


[RequireComponent(typeof(TextMeshProUGUI))]
public class TextLinkOpener : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI textMeshProUGUI;

    private void Awake() => textMeshProUGUI = GetComponent<TextMeshProUGUI>();

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(textMeshProUGUI, eventData.position, null);
        Debug.Log($"{linkIndex}");

        if (linkIndex != -1) 
        { 
            TMP_LinkInfo linkInfo = textMeshProUGUI.textInfo.linkInfo[linkIndex];
            Application.OpenURL(linkInfo.GetLinkID());
        }
    }

}
