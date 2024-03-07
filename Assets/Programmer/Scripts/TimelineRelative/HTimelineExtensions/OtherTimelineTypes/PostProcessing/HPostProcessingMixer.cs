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
        //使用加法来控制后处理某个变量的值，这样很方便依据inputWeight来复原
        if(input.postProcessingType==EnumHPostProcessingType.GlobalVolume)
        {
            HPostProcessingFilters.Instance.SetAttributeAndValueFromTimeline(input.attributes, input.values, input.defaultValues, inputWeight, input.shouldLerp, input.globalVolumeField);
        }
    }

    public void ResetPostProcessingValues(HPostProcessingBehavior input)
    {
        if (input.postProcessingType == EnumHPostProcessingType.GlobalVolume)
        {
            HPostProcessingFilters.Instance.ResetInput(input);
        }
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
            if (inputWeight > 0.00001f)
            {
                //Debug.Log("this clip " + i + " is active");
                //Debug.Log("PPInputWeight"+i+":   "+inputWeight); //每个inputWeight对应的是轨道上的clip的weight，比如混合的时候四个weight可能是0.25 0.75 0 0，如果是一段动画渐进的话，这个weight的和也不一定是1
                //这里的input应该指的就是对应轨道上的那个clip里面的参数
                HandlePostProcessingBehavior(input, inputWeight);
            }
            // else
            // {
            //     Debug.Log("this clip " + i + " should reset");
            //     //应该重置为默认值
            //     ResetPostProcessingValues(input);
            // }
        }
        //Debug.Log("=================================");
       
    }
}
