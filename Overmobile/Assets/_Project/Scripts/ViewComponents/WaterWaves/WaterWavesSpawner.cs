using Cysharp.Threading.Tasks;
using ExtendedExceptions;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ViewComponents.WaterWaves
{
    public sealed class WaterWavesSpawner : MonoBehaviour
    {
        [SerializeField] private WaterWavesInstance _wavePrefab;
        [SerializeField] private int _poolSize = 4;

        [Header("Spawn Area")]
        [SerializeField] private float _spawnRadius = 1f;
        [SerializeField] private Color _spawnRadiusGizmoColor = new Color(r: 0f, g: 1f, b: 1f, a: 0.5f);

        [Header("Spawn Timing")]
        [SerializeField] private int _spawnCountPerBurst = 2;
        [SerializeField] private float _minSpawnInterval = 0.5f;
        [SerializeField] private float _maxSpawnInterval = 2f;

        [Header("Instance Motion")]
        [SerializeField] private float _moveSpeed = 1f;
        [SerializeField] private float _lifetime = 2f;
        [SerializeField] private Vector3 _moveDirection = new Vector3(x: -1f, y: 1f, z: 0f);

        private readonly List<WaterWavesInstance> _pool = new List<WaterWavesInstance>();
        private Vector3 _normalizedMoveDirection;

        private void Awake()
        {
            Validate();

            for (int i = 0; i < _poolSize; i++)
            {
                WaterWavesInstance instance = Instantiate(_wavePrefab, transform);
                instance.gameObject.SetActive(false);
                _pool.Add(instance);
            }
        }

        private void Start()
        {
            RunSpawnLoopAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTaskVoid RunSpawnLoopAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                float delay = Random.Range(_minSpawnInterval, _maxSpawnInterval);

                await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: cancellationToken);

                SpawnBurst(cancellationToken);
            }
        }

        private void SpawnBurst(CancellationToken cancellationToken)
        {
            for (int i = 0; i < _spawnCountPerBurst; i++)
            {
                WaterWavesInstance instance = GetAvailableInstance();

                if (instance == null)
                {
                    break;
                }

                Vector3 spawnPosition = GetRandomSpawnPosition();

                instance
                   .PlayAsync(
                        _normalizedMoveDirection,
                        _moveSpeed,
                        _lifetime,
                        spawnPosition,
                        cancellationToken
                    )
                   .Forget();
            }
        }

        private WaterWavesInstance GetAvailableInstance()
        {
            foreach (WaterWavesInstance instance in _pool)
            {
                if (!instance.gameObject.activeSelf)
                {
                    return instance;
                }
            }

            return null;
        }

        private Vector3 GetRandomSpawnPosition()
        {
            Vector2 offset = Random.insideUnitCircle * _spawnRadius;

            return transform.position + new Vector3(offset.x, offset.y, z: 0f);
        }

        private void Validate()
        {
            Guard.AgainstNull(_wavePrefab, () => new MissingWaterWavesFieldException(nameof(_wavePrefab), gameObject.name));

            _wavePrefab.Validate();

            Guard.AgainstNonPositive(
                _spawnCountPerBurst,
                () => new InvalidWaterWavesValueException(nameof(_spawnCountPerBurst), gameObject.name, _spawnCountPerBurst)
            );

            if (_poolSize < _spawnCountPerBurst)
            {
                throw new InvalidWaterWavesPoolSizeException(_poolSize, _spawnCountPerBurst);
            }

            if (_minSpawnInterval > _maxSpawnInterval)
            {
                throw new InvalidWaterWavesSpawnIntervalException(_minSpawnInterval, _maxSpawnInterval);
            }

            Guard.AgainstNonPositive(
                _moveSpeed,
                () => new InvalidWaterWavesValueException(nameof(_moveSpeed), gameObject.name, _moveSpeed)
            );

            Guard.AgainstNonPositive(
                _lifetime,
                () => new InvalidWaterWavesValueException(nameof(_lifetime), gameObject.name, _lifetime)
            );

            Guard.AgainstNegative(
                _spawnRadius,
                () => new InvalidWaterWavesValueException(nameof(_spawnRadius), gameObject.name, _spawnRadius)
            );

            _normalizedMoveDirection = _moveDirection.sqrMagnitude <= Mathf.Epsilon
                ? Vector3.zero
                : _moveDirection.normalized;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = _spawnRadiusGizmoColor;
            Gizmos.DrawWireSphere(transform.position, _spawnRadius);
        }
    }
}
