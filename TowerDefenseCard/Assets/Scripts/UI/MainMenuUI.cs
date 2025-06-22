using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button startButton;

    private VisualElement visualElement;

    private void Awake() => visualElement = GetComponent<UIDocument>().rootVisualElement;

    private void OnEnable()
    {
        startButton = visualElement.Q<Button>("ButtonStart");
        startButton.clicked += StartButton_clicked;
    }

    private void OnDisable()
    {
        startButton.clicked -= StartButton_clicked;
    }

    private void StartButton_clicked()
    {
        Debug.Log("Load Scene");
        SceneLoader.Instance.LoadNextScene();
    }
}
