using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityVector2;
using Core.AI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace Core.AI
{
    public class YEmemyBlendshapes : YBTEnemyAction
    {
        //应该是在外面设置bs的index和参数的list 以及duration
        public List<int> blendShapeIndex;
        public List<float> blendShapeValue;
        public float duration = 1.0f;
        
        private List<float> blendShapeValueOrigin;
        private List<float> blendShapeValueTarget;
        private List<float> blendShapeValueDelta;
        private float time = 0.0f;
        private bool isStart = false;
        private SkinnedMeshRenderer skinnedMeshRenderer;
        private bool isEnd = false;
        public override void OnStart()
        {
            isEnd = false;
            isStart = false;
            time = 0.0f;
            base.OnStart();
            skinnedMeshRenderer = enemyBT.skinnedMeshRenderer;
            
            blendShapeValueOrigin = new List<float>();
            blendShapeValueTarget = new List<float>();
            blendShapeValueDelta = new List<float>();
            for (int i = 0; i < blendShapeIndex.Count; i++)
            {
                blendShapeValueOrigin.Add(skinnedMeshRenderer.GetBlendShapeWeight(blendShapeIndex[i]));
                blendShapeValueTarget.Add(blendShapeValue[i]);
                blendShapeValueDelta.Add(blendShapeValueTarget[i] - blendShapeValueOrigin[i]);
            }
        }
        public override TaskStatus OnUpdate()
        {
            if (isEnd)
            {
                return TaskStatus.Success;
            }
            if (!isStart)
            {
                isStart = true;
            }
            time += Time.deltaTime;
            for (int i = 0; i < blendShapeIndex.Count; i++)
            {
                //如果这个blendshape他没有
                if (blendShapeIndex[i] >= skinnedMeshRenderer.sharedMesh.blendShapeCount)
                {
                    continue;
                }
                skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex[i], blendShapeValueOrigin[i] + blendShapeValueDelta[i] * time / duration);
            }
            if (time >= duration)
            {
                isEnd = true;
            }
            return TaskStatus.Running;
        }
        
        
        public override void OnEnd()
        {
            base.OnEnd();
            for (int i = 0; i < blendShapeIndex.Count; i++)
            {
                if (blendShapeIndex[i] >= skinnedMeshRenderer.sharedMesh.blendShapeCount)
                {
                    continue;
                }
                skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex[i], blendShapeValueTarget[i]);
                // skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex[i], blendShapeValueOrigin[i]);
            }
        }
    }
}