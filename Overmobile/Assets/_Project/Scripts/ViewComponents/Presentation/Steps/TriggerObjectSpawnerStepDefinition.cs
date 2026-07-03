using Cysharp.Threading.Tasks;
using ExtendedExceptions;
using System;
using System.Threading;
using UnityEngine;

namespace ViewComponents.Presentation
{
    [Serializable]
    public sealed class TriggerObjectSpawnerStepDefinition : PresentationStepDefinition
    {
        [SerializeField] private PresentationObjectSpawner _spawner;
        [SerializeField] private Transform _spawnTransform;

        public override UniTask ExecuteAsync(PresentationContext context, CancellationToken cancellationToken)
        {
            Guard.AgainstNull(
                _spawner,
                () => new InvalidPresentationStepDefinitionException(
                    nameof(TriggerObjectSpawnerStepDefinition),
                    "Spawner is not assigned"
                )
            );

            Guard.AgainstNull(
                _spawnTransform,
                () => new InvalidPresentationStepDefinitionException(
                    nameof(TriggerObjectSpawnerStepDefinition),
                    "Spawn transform is not assigned"
                )
            );

            _spawner.Spawn(_spawnTransform.position, _spawnTransform.rotation);

            return UniTask.CompletedTask;
        }
    }
}
