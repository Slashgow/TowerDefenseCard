using TabUI;
using UnityEngine;

public class UIPageCollection : UIPage
{
    [SerializeField] private TabGroup tabGroup;
    public override void Show()
    {
        base.Show();
        tabGroup.OnTabSelected(tabGroup.DefaultTab);
    }
}
