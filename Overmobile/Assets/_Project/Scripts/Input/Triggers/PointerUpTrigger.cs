using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Input.Triggers
{
    public sealed class PointerUpTrigger
        : MonoBehaviour,
          IPointerUpHandler,
          ITrigger
    {
        public event Action OnTriggered;

        public void OnPointerUp(PointerEventData eventData)
        {
            OnTriggered?.Invoke();
        }
    }
}
