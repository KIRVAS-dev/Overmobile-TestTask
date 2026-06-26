using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Input.Triggers
{
    public sealed class PointerDownTrigger
        : MonoBehaviour,
          IPointerDownHandler,
          ITrigger
    {
        public event Action OnTriggered;

        public void OnPointerDown(PointerEventData eventData)
        {
            OnTriggered?.Invoke();
        }
    }
}
