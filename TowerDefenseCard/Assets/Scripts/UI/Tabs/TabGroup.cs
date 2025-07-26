using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace TabUI
{
    public class TabGroup : MonoBehaviour, IColorable
    {
        [SerializeField] private TabButton defaultTab;
        public TabButton DefaultTab => defaultTab;

        [SerializeField] private bool useColorTheme;
        [SerializeField] private ColorID tabIdleColorID, tabHoverColorID, tabActiveColorID;

        [Header("Settings")]
        [SerializeField, Range(0f, 5f)] private float timeToReachEndColor;
        [SerializeField] private Ease easing;

        protected List<TabButton> tabButtons;
        [SerializeField] protected Color tabIdle, tabHover, tabActive;
        [SerializeField] private List<UIPage> objectsToSwap = new List<UIPage>();

        protected TabButton selectedTab;

        public bool UseColorTheme => useColorTheme;
        public ColorID ColorID => tabIdleColorID;

        private void Start()
        {
            if (useColorTheme)
                OnChangeColorTheme(ColorThemeManager.Instance.CurrentColorTheme);

            ColorThemeManager.OnChangeColorTheme += OnChangeColorTheme;

            OnTabSelected(defaultTab);
        }

        public void Subscribe(TabButton tabButton)
        {
            if(tabButtons == null)
                tabButtons = new List<TabButton>();

            tabButtons.Add(tabButton);
        }

        public void OnTabEnter(TabButton tabButton)
        {
            ResetTabs();

            if(selectedTab != null && tabButton != selectedTab)
                tabButton.Background.DOColor(tabHover, timeToReachEndColor).SetEase(easing).SetUpdate(true);
        }

        public void OnTabExit(TabButton tabButton)
        {
            ResetTabs();
        }

        public virtual void OnTabSelected(TabButton tabButton)
        {
            if (selectedTab != null)
                selectedTab.Deselect();


            selectedTab = tabButton;
            selectedTab.Select();

            ResetTabs();
            tabButton.Background.DOColor(tabActive, timeToReachEndColor).SetEase(easing).SetUpdate(true);

            int index = tabButton.transform.GetSiblingIndex();
            for(int i = 0; i < objectsToSwap.Count; i++)
            {
                if(i == index)
                    objectsToSwap[i].Show();
                else
                    objectsToSwap[i].Hide();
            }
        }

        public virtual void ResetTabs()
        {
            foreach(TabButton tabButton in tabButtons)
            {
                if(selectedTab != null && tabButton == selectedTab)
                    continue;

                tabButton.Background.DOColor(tabIdle, timeToReachEndColor).SetEase(easing).SetUpdate(true);
            }
        }

        public void OnChangeColorTheme(ColorTheme colorTheme)
        {
            tabIdle = ColorThemeManager.Instance.GetColorByThemeAndID(colorTheme, tabIdleColorID);
            tabHover = ColorThemeManager.Instance.GetColorByThemeAndID(colorTheme, tabHoverColorID);
            tabActive = ColorThemeManager.Instance.GetColorByThemeAndID(colorTheme, tabActiveColorID);
        }

        private void OnDestroy()
        {
            ColorThemeManager.OnChangeColorTheme -= OnChangeColorTheme;
        }
    }

}
