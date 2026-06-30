using ExtendedExceptions;

namespace ViewComponents.Entity
{
    public sealed class MissingEntityKeyException : ExtendedException
    {
        public MissingEntityKeyException(string objectName)
            : base("entity-key-1", $"Entity key is not assigned on '{objectName}'") { }
    }
}
