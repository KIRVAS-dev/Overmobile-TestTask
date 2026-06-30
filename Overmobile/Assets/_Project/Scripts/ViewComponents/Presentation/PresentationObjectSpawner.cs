using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ViewComponents.Presentation
{
    [DisallowMultipleComponent]
    public sealed class PresentationObjectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject[] _prefabs = Array.Empty<GameObject>();
        [SerializeField] private bool _spawnRandomly;
        [SerializeField] private int _spawnCount = 1;

        private void Awake()
        {
            Validate();
        }

        public void Spawn(Vector3 position, Quaternion rotation)
        {
            if (_spawnRandomly)
            {
                for (int i = 0; i < _spawnCount; i++)
                {
                    GameObject prefab = _prefabs[Random.Range(minInclusive: 0, _prefabs.Length)];
                    Instantiate(prefab, position, rotation);
                }

                return;
            }

            foreach (GameObject prefab in _prefabs)
            {
                Instantiate(prefab, position, rotation);
            }
        }

        private void Validate()
        {
            if (_prefabs == null
             || _prefabs.Length == 0)
            {
                throw new MissingPresentationObjectSpawnerPrefabsException(gameObject.name);
            }

            for (int i = 0; i < _prefabs.Length; i++)
            {
                if (_prefabs[i] == null)
                {
                    throw new InvalidPresentationObjectSpawnerPrefabException(gameObject.name, i);
                }
            }

            if (_spawnRandomly && _spawnCount <= 0)
            {
                throw new InvalidPresentationObjectSpawnerCountException(gameObject.name, _spawnCount);
            }
        }
    }
}
