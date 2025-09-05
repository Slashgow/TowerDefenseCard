using System;

public interface IUISelectable<T>
{
    event Action<T> OnSelectEvent;
    public void OnSelect(T data);
    public T GetSelectableData();
}
