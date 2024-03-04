public abstract class HPlayerBaseState
{
    protected HPlayerStateMachine _ctx;
    protected HPlayerStateFactory _factory;
    
    protected HPlayerBaseState _currentSubState;
    protected HPlayerBaseState _currentSuperState;

    protected bool _isRootState = false;
    
    public HPlayerBaseState(HPlayerStateMachine currentContext, HPlayerStateFactory playerStateFactory)
    {
        _ctx = currentContext;
        _factory = playerStateFactory;
    }
    
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubState();

    public void UpdateStates()
    {
        UpdateState();
        if (_currentSubState != null)
        {
            _currentSubState.UpdateStates();
        }
    }

    // public void ExitStates()
    // {
    //     ExitState();
    //     if(_currentSubState != null)
    //     {
    //         _currentSubState.ExitStates();
    //     }
    // }

    protected void SwitchState(HPlayerBaseState newState)
    {
        //current state exits state
        ExitState();
        
        //new state enters state
        newState.EnterState();

        if (_isRootState)
        {
            //switch current state of context
            _ctx.CurrentState = newState;
        } else if (_currentSuperState != null)
        {
            //switch current substate of superstate
            _currentSuperState.SetSubState(newState);
        }
        
    }

    protected void SetSuperState(HPlayerBaseState newSuperstate)
    {
        _currentSuperState = newSuperstate;
    }

    protected void SetSubState(HPlayerBaseState newSubstate)
    {
        _currentSubState = newSubstate;
        newSubstate.SetSuperState(this);
    }

}
