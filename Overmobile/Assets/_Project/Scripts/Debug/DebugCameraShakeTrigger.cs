#if UNITY_EDITOR
using Core.Bootstrap;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using ViewComponents.Camera;

namespace ProjectDebug
{
    public sealed class DebugCameraShakeTrigger : MonoBehaviour
    {
        [SerializeField] private Key _hotkey = Key.C;

        private ICameraShakeView _cameraShakeView;

        private void Update()
        {
            if (!DebugHotkey.WasPressedThisFrame(_hotkey))
            {
                return;
            }

            if (!TryEnsureCameraShakeView())
            {
                return;
            }

            _cameraShakeView.PlayShake();
        }

        private bool TryEnsureCameraShakeView()
        {
            if (_cameraShakeView != null)
            {
                return true;
            }

            CoreScope coreScope = FindAnyObjectByType<CoreScope>(FindObjectsInactive.Exclude);

            if (coreScope == null)
            {
                return false;
            }

            _cameraShakeView = coreScope.Container.Resolve<ICameraShakeView>();

            return true;
        }
    }
}
#endif
