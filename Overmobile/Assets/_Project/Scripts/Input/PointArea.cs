using Input.Triggers;
using UnityEngine;

namespace Input
{
    public sealed class PointArea : MonoBehaviour
    {
        public ITrigger PointerDown => _pointerDownTrigger;
        public ITrigger PointerUp => _pointerUpTrigger;
        public ITrigger PointerExit => _pointerExitTrigger;
        public ITrigger PointerEnter => _pointerEnterTrigger;

        [SerializeField] private PointerDownTrigger _pointerDownTrigger;
        [SerializeField] private PointerUpTrigger _pointerUpTrigger;
        [SerializeField] private PointerExitTrigger _pointerExitTrigger;
        [SerializeField] private PointerEnterTrigger _pointerEnterTrigger;
    }
}
