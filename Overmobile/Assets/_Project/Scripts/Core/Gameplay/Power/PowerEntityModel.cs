using R3;

namespace Core.Gameplay.Power
{
    public sealed class PowerEntityModel : IPowerEntity
    {
        private readonly ReactiveProperty<int> _power;

        public PowerEntityModel(string entityId, int initialPower)
        {
            EntityId = entityId;
            _power = new ReactiveProperty<int>(initialPower);
        }

        public ReadOnlyReactiveProperty<int> Power => _power;
        public string EntityId { get; }
        public bool IsResolved { get; private set; }

        public void AddPower(int amount)
        {
            _power.Value += amount;
        }

        public void MarkResolved()
        {
            IsResolved = true;
        }
    }
}
