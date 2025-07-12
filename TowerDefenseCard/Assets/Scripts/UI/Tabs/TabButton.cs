using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TabUI
{
    [RequireComponent(typeof(Image))]
    public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private TabGroup tabGroup;

        public UnityEvent onTabSelected;
        public UnityEvent onTabDeselected;
        
        private Image background;
        public Image Background => background;

        private void Awake()
        {
            background = GetComponent<Image>();
            tabGroup.Subscribe(this);
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            tabGroup.OnTabSelected(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            tabGroup.OnTabEnter(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tabGroup.OnTabExit(this);
        }

        public void Select() => onTabSelected.Invoke();
        public void Deselect() => onTabDeselected.Invoke();
    }
}

