namespace Core.Gameplay.Character
{
    public interface IActiveCharacterViewProvider
    {
        ICharacterView ActiveCharacterView { get; }
    }
}
