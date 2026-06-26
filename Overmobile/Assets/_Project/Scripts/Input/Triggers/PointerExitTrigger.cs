using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Input.Triggers
{
    public sealed class PointerExitTrigger
        : MonoBehaviour,
          IPointerExitHandler,
          ITrigger
    {
        public event Action OnTriggered;

        public void OnPointerExit(PointerEventData eventData)
        {
            OnTriggered?.Invoke();
        }
    }
}
