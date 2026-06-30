using Cysharp.Threading.Tasks;
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
            if (_spawner == null)
            {
                throw new InvalidPresentationStepDefinitionException(
                    nameof(TriggerObjectSpawnerStepDefinition),
                    "Spawner is not assigned"
                );
            }

            Transform spawnSource = _spawnTransform != null
                ? _spawnTransform
                : context.Owner.transform;

            _spawner.Spawn(spawnSource.position, spawnSource.rotation);

            return UniTask.CompletedTask;
        }
    }
}
