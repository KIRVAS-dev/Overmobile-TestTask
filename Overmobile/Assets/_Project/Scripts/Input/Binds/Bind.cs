using System;

namespace Input.Binds
{
    public sealed class Bind
    {
        private readonly ITrigger _trigger;

        private Action _onTriggered;

        public event Action OnTriggered
        {
            add => _onTriggered += value;
            remove => _onTriggered -= value;
        }

        public Bind(ITrigger trigger)
        {
            _trigger = trigger;
        }

        public void Enable()
        {
            _trigger.OnTriggered += HandleTrigger;
        }

        public void Disable()
        {
            _trigger.OnTriggered -= HandleTrigger;
            _onTriggered = null;
        }

        private void HandleTrigger()
        {
            _onTriggered?.Invoke();
        }
    }
}
