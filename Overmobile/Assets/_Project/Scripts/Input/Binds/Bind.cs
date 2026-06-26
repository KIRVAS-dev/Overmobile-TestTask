using System;

namespace Input.Binds
{
    public sealed class Bind
    {
        public event Action OnTriggered;

        private readonly ITrigger _trigger;

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
        }

        private void HandleTrigger()
        {
            OnTriggered?.Invoke();
        }
    }
}
