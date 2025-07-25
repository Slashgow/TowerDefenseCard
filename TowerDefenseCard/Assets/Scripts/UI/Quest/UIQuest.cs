using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIQuest : MonoBehaviour
{
    [SerializeField] private Toggle toggleIsDone;
    [SerializeField] private TextMeshProUGUI questDescriptionText;

    public void Setup(string questDescription)
    {
        questDescriptionText.text = questDescription;  
    }
}
