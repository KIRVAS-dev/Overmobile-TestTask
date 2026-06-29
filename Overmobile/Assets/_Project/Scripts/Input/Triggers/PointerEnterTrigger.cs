using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Input.Triggers
{
    public sealed class PointerEnterTrigger
        : MonoBehaviour,
          IPointerEnterHandler,
          ITrigger
    {
        public event Action OnTriggered;

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnTriggered?.Invoke();
        }
    }
}
