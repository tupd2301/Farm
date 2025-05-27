namespace AthenaFramework.GameState
{
    public class StateManager : IStateManager
    {
        private IGameState _currentGameState;

        public void ChangeState(IGameState stateToChange)
        {
            _currentGameState?.Deactivate();
            _currentGameState = stateToChange;
            _currentGameState.Activate();
        }
    }
}
