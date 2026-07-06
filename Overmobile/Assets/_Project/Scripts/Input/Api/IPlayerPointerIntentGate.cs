namespace Input
{
    public interface IPlayerPointerIntentGate
    {
        void RegisterPendingPointerDown(IPendingPointerDownTrigger trigger);
    }
}
