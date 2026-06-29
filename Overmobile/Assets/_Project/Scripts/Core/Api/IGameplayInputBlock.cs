namespace Core
{
    public interface IGameplayInputBlock
    {
        bool IsBlocked { get; }
        void Block();
        void Unblock();
    }
}
