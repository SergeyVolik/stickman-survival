namespace Prototype
{
    public interface IStateMachine
    {
        public IState CurrentState { get; }
        public void ChangeState(IState state);
    }
}