using UnityEngine;

namespace ViewComponents.Camera
{
    public interface ICameraConfinerView
    {
        void BeginBoundsFollow();
        void UpdateBoundsFollow(Vector3 followPosition);
        void EndBoundsFollow();
    }
}
