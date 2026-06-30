using ExtendedExceptions;

namespace ViewComponents.Entity
{
    public sealed class MissingEntityKeyException : ExtendedException
    {
        public MissingEntityKeyException(string objectName)
            : base("entity-key-1", $"Entity key is not assigned on '{objectName}'") { }
    }

    public sealed class MissingEntityIdException : ExtendedException
    {
        public MissingEntityIdException(string objectName)
            : base("entity-id-1", $"Entity id is not assigned on '{objectName}'") { }
    }
}
