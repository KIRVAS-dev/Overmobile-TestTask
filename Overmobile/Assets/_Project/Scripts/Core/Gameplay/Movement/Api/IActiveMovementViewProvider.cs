namespace Core.Gameplay.Movement
{
    public interface IActiveMovementViewProvider
    {
        IMovementView ActiveMovementView { get; }
    }
}
