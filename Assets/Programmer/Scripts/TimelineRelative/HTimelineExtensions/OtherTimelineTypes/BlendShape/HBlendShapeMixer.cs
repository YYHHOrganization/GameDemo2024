using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class HBlendShapeMixer : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        SkinnedMeshRenderer skinnedMeshRenderer = playerData as SkinnedMeshRenderer;
        if (skinnedMeshRenderer == null)
        {
            //Debug.Log("skinnedMeshRenderer is null");
            return;
        }
        
        int inputCount = playable.GetInputCount();
        //int inputCount =yPlanningTable.Instance.isMoveList.Count;//这个代表的是当前一共会有多少个clip生效
        
        // Debug.Log("inputCount: " + inputCount);
        float totalWeight = 0;
        
        for (int i = 0; i < inputCount; i++)
        {
            //之前bug“这里没有每个blendshape单独改 而是全给改了。这里是一个bug 应该是获取当前的这个轨道上的权重 然后再去改当前轨道的blendshape 而不要改到全部的bs
            float inputWeight = playable.GetInputWeight(i);
            totalWeight += inputWeight;
            if (inputWeight > 0.001f)
            {
                //Debug.Log("i"+i+" inputWeight: " + inputWeight + " totalWeight: " + totalWeight);
                //被改了，当前改动的这个轨道的所有的blendshapeIndex应该被改  应该记录下来
                ScriptPlayable<HBlendShapeBehavior> inputPlayable = (ScriptPlayable<HBlendShapeBehavior>)playable.GetInput(i);
                HBlendShapeBehavior input = inputPlayable.GetBehaviour();
                for (int j = 0; j < input.blendShapeValue.Count; j++)
                {
                    // skinnedMeshRenderer.SetBlendShapeWeight(input.blendShapeIndex[j],  totalWeight * input.blendShapeValue[j]);
                    skinnedMeshRenderer.SetBlendShapeWeight(input.blendShapeIndex[j],  inputWeight * input.blendShapeValue[j]);
                }
                //问题在于
            }
            
//            Debug.Log("totalWeight"+totalWeight);
            //【别删】如果全部没改 totalWeight是0  那么需要重置blendshape  
            if(i==inputCount-1&&Mathf.Abs(totalWeight) < 0.001f)//防止是负数  但是感觉有点废 感觉没这个也行吧 bs都维持在0.001也看不出来 这个先关闭 有需要在开启
            {
                ScriptPlayable<HBlendShapeBehavior> inputPlayable = (ScriptPlayable<HBlendShapeBehavior>)playable.GetInput(i);
                HBlendShapeBehavior input = inputPlayable.GetBehaviour();
                for (int j = 0; j < input.blendShapeValue.Count; j++)
                {
                    skinnedMeshRenderer.SetBlendShapeWeight(input.blendShapeIndex[j],  0 * input.blendShapeValue[j]);
                }
                //Debug.Log("重置blendshape"+"totalWeight"+totalWeight);
            }
            
        }
        
    }

}
