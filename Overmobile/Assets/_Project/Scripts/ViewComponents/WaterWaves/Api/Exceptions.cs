using ExtendedExceptions;

namespace ViewComponents.WaterWaves
{
    public sealed class MissingWaterWavesFieldException : ExtendedException
    {
        public MissingWaterWavesFieldException(string fieldName, string objectName)
            : base("water-waves-1", $"Required field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    public sealed class InvalidWaterWavesValueException : ExtendedException
    {
        public InvalidWaterWavesValueException(
            string fieldName,
            string objectName,
            float value)
            : base("water-waves-2", $"Field '{fieldName}' on '{objectName}' has invalid value: {value}") { }

        public InvalidWaterWavesValueException(
            string fieldName,
            string objectName,
            int value)
            : base("water-waves-2", $"Field '{fieldName}' on '{objectName}' has invalid value: {value}") { }
    }

    public sealed class InvalidWaterWavesSpritesException : ExtendedException
    {
        public InvalidWaterWavesSpritesException()
            : base("water-waves-3", "Water waves instance config sprites array is empty or contains null entries") { }
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

    public sealed class InvalidWaterWavesFadeDurationException : ExtendedException
    {
        public InvalidWaterWavesFadeDurationException(string fadeDurationName, float fadeDuration)
            : base(
                "water-waves-6",
                $"Water waves instance config {fadeDurationName} must be zero or greater, got {fadeDuration}"
            ) { }
    }
}
