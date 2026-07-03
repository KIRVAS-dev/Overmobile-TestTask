using Cysharp.Threading.Tasks;
using System.Threading;

namespace Core.Gameplay.Attack
{
    public interface IAttackView
    {
        UniTask PlayAttackAsync(CancellationToken cancellationToken);
    }
}
