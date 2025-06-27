public interface IUpgradable
{
    void ApplyUpgrade(UpgradeData upgrade);
    bool CanApplyUpgrade(UpgradeData upgrade);
    bool RemoveUpgrade(string upgradeName);
    UpgradeData[] GetAppliedUpgrades();
}
