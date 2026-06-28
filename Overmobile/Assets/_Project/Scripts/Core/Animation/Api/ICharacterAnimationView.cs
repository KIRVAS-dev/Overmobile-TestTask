using Cysharp.Threading.Tasks;
using System.Threading;

namespace Core.Animation
{
    public interface ICharacterAnimationView
    {
        CharacterAnimationSlot CurrentAnimationSlot { get; }

        void SetIsMoving(bool value);

        void FireAnimation(CharacterAnimationSlot slot);

        UniTask FireAnimationAsync(CharacterAnimationSlot slot, CancellationToken cancellationToken);
    }
}
