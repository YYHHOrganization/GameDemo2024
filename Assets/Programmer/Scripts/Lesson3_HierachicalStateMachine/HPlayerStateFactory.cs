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
        die,
        withwater,
        swimIdle,
        swimSlow,
        swimFast,
        floatOnWater,
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
        _states[PlayerStates.die] = new YPlayerDieState(_context, this);
        _states[PlayerStates.withwater] = new HPlayerWithWaterState(_context, this);
        _states[PlayerStates.swimIdle] = new HPlayerSwimIdleState(_context, this);
        _states[PlayerStates.swimSlow] = new HPlayerSwimSlowState(_context, this);
        _states[PlayerStates.swimFast] = new HPlayerSwimFastState(_context, this);
        _states[PlayerStates.floatOnWater] = new HPlayerWaterFloatState(_context, this);
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
    
    public HPlayerBaseState Die()
    {
        return _states[PlayerStates.die];
    }
    
    public HPlayerBaseState WithWater()
    {
        return _states[PlayerStates.withwater];
    }
    
    public HPlayerBaseState SwimIdle()
    {
        return _states[PlayerStates.swimIdle];
    }
    
    public HPlayerBaseState SwimSlow()
    {
        return _states[PlayerStates.swimSlow];
    }
    
    public HPlayerBaseState SwimFast()
    {
        return _states[PlayerStates.swimFast];
    }
    
    public HPlayerBaseState FloatOnWater()
    {
        return _states[PlayerStates.floatOnWater];
    }
    
}
