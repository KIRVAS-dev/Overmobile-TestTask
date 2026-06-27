namespace Core.Gameplay.Player
{
    public interface IPlayerUpgradeView
    {
        int CurrentTierIndex { get; }

        bool CanUpgrade { get; }

        void Upgrade();
    }
}
