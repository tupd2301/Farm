namespace AthenaFramework.GameState
{
    public interface IStateManager
    {
        void ChangeState(IGameState stateToChange);
    }
}
