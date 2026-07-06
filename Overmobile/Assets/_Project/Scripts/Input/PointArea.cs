using ExtendedExceptions;
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

        public void BindPlayerPointerInput(IPlayerPointerIntentGate intentGate)
        {
            Guard.AgainstNull(intentGate, () => new MissingPlayerPointerInputForPointAreaBindException(gameObject.name));

            _pointerDownTrigger.Configure(intentGate);
        }

        private void Awake()
        {
            Validate();
        }

        private void Validate()
        {
            Guard.AgainstNull(
                _pointerDownTrigger,
                () => new MissingPointAreaFieldException(nameof(_pointerDownTrigger), gameObject.name)
            );

            Guard.AgainstNull(
                _pointerUpTrigger,
                () => new MissingPointAreaFieldException(nameof(_pointerUpTrigger), gameObject.name)
            );

            Guard.AgainstNull(
                _pointerExitTrigger,
                () => new MissingPointAreaFieldException(nameof(_pointerExitTrigger), gameObject.name)
            );

            Guard.AgainstNull(
                _pointerEnterTrigger,
                () => new MissingPointAreaFieldException(nameof(_pointerEnterTrigger), gameObject.name)
            );
        }
    }
}
