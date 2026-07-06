using ExtendedExceptions;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Input.Triggers
{
    public sealed class PointerDownTrigger
        : MonoBehaviour,
          IPendingPointerDownTrigger,
          IPointerDownHandler
    {
        private IPlayerPointerIntentGate _intentGate;

        public event Action OnTriggered;

        public void Configure(IPlayerPointerIntentGate intentGate)
        {
            _intentGate = intentGate;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Guard.AgainstNull(_intentGate, () => new MissingPointerDownIntentGateException(gameObject.name));

            _intentGate.RegisterPendingPointerDown(this);
        }

        public void InvokeConfirmed()
        {
            OnTriggered?.Invoke();
        }
    }
}
