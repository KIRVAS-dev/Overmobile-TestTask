using R3;

namespace Core.Gameplay.Power
{
    public sealed class PowerEntityModel : IPowerEntity
    {
        private readonly ReactiveProperty<int> _power;
        private readonly ReactiveProperty<bool> _isResolved = new ReactiveProperty<bool>(false);

        public PowerEntityModel(string entityId, int initialPower)
        {
            EntityId = entityId;
            _power = new ReactiveProperty<int>(initialPower);
        }

        public ReadOnlyReactiveProperty<int> Power => _power;
        public ReadOnlyReactiveProperty<bool> IsResolved => _isResolved;
        public string EntityId { get; }

        public void AddPower(int amount)
        {
            _power.Value += amount;
        }

        public void MarkResolved()
        {
            _isResolved.Value = true;
        }
    }
}
