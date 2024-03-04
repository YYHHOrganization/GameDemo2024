using System.Collections.Generic;

public class HPlayerStateFactoryNew
{
    private HPlayerStateMachineNew _context;
    enum PlayerStatesAuto //自动执行状态下的一些动作状态
    {
        idle,
        walk,
        run,
        grounded,
        jump,
        fall,
        crawl, //爬行
        userdef, //用户自定义的状态，用于扩展，感觉可以用于比如一些打招呼之类的动作
    }

    private Dictionary<PlayerStatesAuto, HPlayerBaseStateNew> _states =
        new Dictionary<PlayerStatesAuto, HPlayerBaseStateNew>(); 

    public HPlayerStateFactoryNew(HPlayerStateMachineNew context)
    {
        _context = context;
        //todo: new 出一些新的状态类
        
    }
}
