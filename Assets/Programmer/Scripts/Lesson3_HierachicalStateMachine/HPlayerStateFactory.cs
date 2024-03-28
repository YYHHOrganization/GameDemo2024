using System.Collections.Generic;

public class HPlayerStateFactory
{
    private HPlayerStateMachine _context;
    Dictionary<PlayerStates, HPlayerBaseState> _states = new Dictionary<PlayerStates, HPlayerBaseState>();

    enum PlayerStates
    {
        idle,
        walk,
        run,
        grounded,
        jump,
        fall,
        Skill1,
    }
    
    public HPlayerStateFactory(HPlayerStateMachine context)
    {
        _context = context;
        _states[PlayerStates.idle] = new HPlayerIdleState(_context, this);
        _states[PlayerStates.walk] = new PlayerWalkState(_context, this);
        _states[PlayerStates.run] = new PlayerRunState(_context, this);
        _states[PlayerStates.jump] = new HPlayerJumpState(_context, this);
        _states[PlayerStates.grounded] = new HPlayerGroundedState(_context, this);
        _states[PlayerStates.fall] = new HPlayerFallState(_context, this);
        _states[PlayerStates.Skill1] = new HPlayerSkill1State(_context, this);
    }

    public HPlayerBaseState Idle()
    {
        return _states[PlayerStates.idle];
    }

    public HPlayerBaseState Walk()
    {
        return _states[PlayerStates.walk];
    }

    public HPlayerBaseState Run()
    {
        return _states[PlayerStates.run];
    }

    public HPlayerBaseState Jump()
    {
        return _states[PlayerStates.jump];
    }

    public HPlayerBaseState Grounded()
    {
        return _states[PlayerStates.grounded];
    }

    public HPlayerBaseState Fall()
    {
        return _states[PlayerStates.fall];
    }
    
    public HPlayerBaseState Skill1()
    {
        return _states[PlayerStates.Skill1];
    }
}
