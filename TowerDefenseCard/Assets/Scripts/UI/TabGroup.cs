using System.Collections.Generic;
using UnityEngine;

namespace TabUI
{
    public class TabGroup : MonoBehaviour
    {
        [SerializeField] private TabButton defaultTab;
        private List<TabButton> tabButtons;
        [SerializeField] private Color tabIdle, tabHover, tabActive;
        [SerializeField] private List<UIPage> objectsToSwap = new List<UIPage>();

        private TabButton selectedTab;

        private void Start()
        {
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
                tabButton.Background.color = tabHover;
        }

        public void OnTabExit(TabButton tabButton)
        {
            ResetTabs();
        }

        public void OnTabSelected(TabButton tabButton)
        {
            if (selectedTab != null)
                selectedTab.Deselect();


            selectedTab = tabButton;
            selectedTab.Select();

            ResetTabs();
            tabButton.Background.color = tabActive;

            int index = tabButton.transform.GetSiblingIndex();
            for(int i = 0; i < objectsToSwap.Count; i++)
            {
                if(i == index)
                    objectsToSwap[i].Show();
                else
                    objectsToSwap[i].Hide();
            }
        }

        public void ResetTabs()
        {
            foreach(TabButton tabButton in tabButtons)
            {
                if(selectedTab != null && tabButton == selectedTab)
                    continue;

                tabButton.Background.color = tabIdle;
            }
        }
    }

}
