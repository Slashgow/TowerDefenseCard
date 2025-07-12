using UnityEngine;

namespace TabUI
{
    public class TabGoupSpriteChange : TabGroup
    {
        [SerializeField] private Sprite idleSprite, selecteSprite;

        public override void OnTabSelected(TabButton tabButton)
        {
            base.OnTabSelected(tabButton);

            tabButton.Background.sprite = selecteSprite;
        }

        public override void ResetTabs()
        {
            foreach (TabButton tabButton in tabButtons)
            {
                if (selectedTab != null && tabButton == selectedTab)
                    continue;

                tabButton.Background.color = tabIdle;
                tabButton.Background.sprite = idleSprite;
            }
        }
    }
}

