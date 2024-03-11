using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Playables;

public class HPostProcessingMixer : PlayableBehaviour
{
    public void HandlePostProcessingBehavior(HPostProcessingBehavior input, float inputWeight)
    {
        HPostProcessingFilters.Instance.SetAttributeAndValueFromTimelineNew(input, inputWeight);
    }
    
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        //Debug.Log("==================test==================");
        //Debug.Log("now in one tick processFrame");
        //playerData指的是轨道track上绑定的物体，是在Track对应脚本里绑定的
        //playable是什么呢？The Playable that owns the current PlayableBehaviour.
        
        //Debug.Log(playable.GetPlayableType()); //output:HPostProcessingMixer
        //Debug.Log("PPPlayableOutputCount"+playable.GetOutputCount()); //即使两个轨道混在一起，这个输出也是1
        //Debug.Log("PPPlayableInputCount"+playable.GetInputCount());  //输出结果一直是4，应该指的是轨道上有多少个clip
        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<HPostProcessingBehavior> inputPlayable =
                (ScriptPlayable<HPostProcessingBehavior>)playable.GetInput(i);
            HPostProcessingBehavior input = inputPlayable.GetBehaviour();
            //Debug.Log("inputWeight: " + inputWeight + " for clip:" + i);
            if ((inputWeight > 0.00001f))  // && (inputWeight < 1.0f), todo：有需要再优化，没需要算了
            {
                //Debug.Log("this clip " + i + " is active");
                //Debug.Log("PPInputWeight"+i+":   "+inputWeight); //每个inputWeight对应的是轨道上的clip的weight，比如混合的时候四个weight可能是0.25 0.75 0 0，如果是一段动画渐进的话，这个weight的和也不一定是1
                //这里的input应该指的就是对应轨道上的那个clip里面的参数
                HandlePostProcessingBehavior(input, inputWeight);
            }
            //当离开轨道的时候，调用的逻辑写在了Behavior里，会重置为策划表中写的默认值
        }
        //Debug.Log("=================================");
       
    }
}
