using ExtendedExceptions;

namespace ViewComponents.WaterWaves
{
    public sealed class MissingWaterWavesWavePrefabException : ExtendedException
    {
        public MissingWaterWavesWavePrefabException(string objectName)
            : base("water-waves-1", $"Water waves wave prefab is not assigned on '{objectName}'") { }
    }

    public sealed class InvalidWaterWavesSpritesException : ExtendedException
    {
        public InvalidWaterWavesSpritesException()
            : base("water-waves-2", "Water waves instance config sprites array is empty or contains null entries") { }
    }

    public sealed class InvalidWaterWavesSpawnCountException : ExtendedException
    {
        public InvalidWaterWavesSpawnCountException(int spawnCountPerBurst)
            : base(
                "water-waves-3",
                $"Water waves spawner spawn count per burst must be greater than zero, got {spawnCountPerBurst}"
            ) { }
    }

    public sealed class InvalidWaterWavesPoolSizeException : ExtendedException
    {
        public InvalidWaterWavesPoolSizeException(int poolSize, int spawnCountPerBurst)
            : base(
                "water-waves-4",
                $"Water waves spawner pool size must be at least spawn count per burst ({spawnCountPerBurst}), got {poolSize}"
            ) { }
    }

    public sealed class InvalidWaterWavesSpawnIntervalException : ExtendedException
    {
        public InvalidWaterWavesSpawnIntervalException(float minSpawnInterval, float maxSpawnInterval)
            : base(
                "water-waves-5",
                $"Water waves spawner min spawn interval must not exceed max spawn interval, got {minSpawnInterval} and {maxSpawnInterval}"
            ) { }
    }

    public sealed class InvalidWaterWavesMoveSpeedException : ExtendedException
    {
        public InvalidWaterWavesMoveSpeedException(float moveSpeed)
            : base("water-waves-6", $"Water waves spawner move speed must be greater than zero, got {moveSpeed}") { }
    }

    public sealed class InvalidWaterWavesLifetimeException : ExtendedException
    {
        public InvalidWaterWavesLifetimeException(float lifetime)
            : base("water-waves-7", $"Water waves spawner lifetime must be greater than zero, got {lifetime}") { }
    }

    public sealed class InvalidWaterWavesSpawnRadiusException : ExtendedException
    {
        public InvalidWaterWavesSpawnRadiusException(float spawnRadius)
            : base("water-waves-8", $"Water waves spawner spawn radius must be zero or greater, got {spawnRadius}") { }
    }

    public sealed class InvalidWaterWavesFadeDurationException : ExtendedException
    {
        public InvalidWaterWavesFadeDurationException(string fadeDurationName, float fadeDuration)
            : base(
                "water-waves-9",
                $"Water waves instance config {fadeDurationName} must be zero or greater, got {fadeDuration}"
            ) { }
    }

    public sealed class MissingWaterWavesSpriteRendererException : ExtendedException
    {
        public MissingWaterWavesSpriteRendererException(string objectName)
            : base("water-waves-11", $"Water waves sprite renderer is not assigned on '{objectName}'") { }
    }

    public sealed class MissingWaterWavesInstanceConfigException : ExtendedException
    {
        public MissingWaterWavesInstanceConfigException(string objectName)
            : base("water-waves-12", $"Water waves instance config is not assigned on '{objectName}' prefab") { }
    }
}
