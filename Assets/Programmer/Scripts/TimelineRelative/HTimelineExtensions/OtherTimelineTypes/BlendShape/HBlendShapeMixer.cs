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
            return;

        int inputCount = playable.GetInputCount();
        float totalWeight = 0;
        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            totalWeight += inputWeight;

            ScriptPlayable<HBlendShapeBehavior> inputPlayable = (ScriptPlayable<HBlendShapeBehavior>)playable.GetInput(i);
            HBlendShapeBehavior input = inputPlayable.GetBehaviour();
            for (int j = 0; j < input.blendShapeValue.Count; j++)
            {
                skinnedMeshRenderer.SetBlendShapeWeight(input.blendShapeIndex[j],  inputWeight * input.blendShapeValue[j]);
            }
            
        }
    }
}
