using System;

public interface IUnlockable
{
    bool IsUnlocked { get; }
    void Unlock();

    public event Action OnUnlock;
    public string LockUIDescription { get; }
}
