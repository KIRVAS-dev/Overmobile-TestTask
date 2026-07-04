using UnityEngine;

namespace Rendering.TargetSelection
{
    public static class TargetSelectionHighlightActivity
    {
        private static int _activeHighlightCount;

        public static bool HasActiveHighlights => _activeHighlightCount > 0;

        public static void Register()
        {
            _activeHighlightCount++;
        }

        public static void Unregister()
        {
            if (_activeHighlightCount <= 0)
            {
                throw new UnbalancedTargetSelectionHighlightActivityException();
            }

            _activeHighlightCount--;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOnDomainReload()
        {
            _activeHighlightCount = 0;
        }
    }
}
