using ExtendedExceptions;
using System;
using UnityEngine;
using VContainer;

namespace ViewComponents.Camera
{
    [DisallowMultipleComponent]
    public sealed class CameraLateUpdateDriver : MonoBehaviour
    {
        private ICameraBoundsFrameUpdater _boundsFrameUpdater;
        private ICameraZoomFrameUpdater _zoomFrameUpdater;

        [Inject]
        public void Construct(ICameraBoundsFrameUpdater boundsFrameUpdater, ICameraZoomFrameUpdater zoomFrameUpdater)
        {
            _boundsFrameUpdater = boundsFrameUpdater;
            _zoomFrameUpdater = zoomFrameUpdater;
        }

        private void Awake()
        {
            Validate();
        }

        private void LateUpdate()
        {
            _boundsFrameUpdater.TickBoundsFrame();
            _zoomFrameUpdater.TickZoomFrame();
        }

        private void Validate()
        {
            Guard.AgainstNull(
                _boundsFrameUpdater,
                () => new MissingCameraLateUpdateDriverFieldException(nameof(_boundsFrameUpdater), gameObject.name)
            );

            Guard.AgainstNull(
                _zoomFrameUpdater,
                () => new MissingCameraLateUpdateDriverFieldException(nameof(_zoomFrameUpdater), gameObject.name)
            );
        }
    }
}
