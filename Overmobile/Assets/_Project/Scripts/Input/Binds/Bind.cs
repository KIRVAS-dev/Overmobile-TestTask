using System;

namespace Input.Binds
{
    public sealed class Bind
    {
        private Action onTriggered;

        private readonly ITrigger _trigger;

        public event Action OnTriggered
        {
            add => onTriggered += value;
            remove => onTriggered -= value;
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
            onTriggered = null;
        }

        private void HandleTrigger()
        {
            onTriggered?.Invoke();
        }
    }
}
